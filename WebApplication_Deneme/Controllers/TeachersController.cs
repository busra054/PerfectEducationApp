using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_Domain.Entities;
using WebApplication_Infrastructure.Data;

namespace WebApplication_Deneme.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;

        public TeachersController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            UserManager<User> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // TeachersController.cs
        [HttpGet]
        public async Task<IActionResult> ViewSubmissions(int assignmentId)
        {
            // GetUserId string döndürdüğü için int'e parse ediyoruz
            var currentUserId = int.Parse(_userManager.GetUserId(User));

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == currentUserId); // Artık int == int

            if (teacher == null) return Forbid();

            // Ödevin kursunu ve öğretmen ilişkisini kontrol et
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == assignmentId && a.Course.TeacherId == teacher.Id);

            if (assignment == null) return NotFound();

            // Gönderimleri öğrenci bilgileriyle birlikte yükle
            var submissions = await _context.AssignmentSubmissions
                .Include(s => s.Student.User)
                .Include(s => s.Assignment)
                .Where(s => s.AssignmentId == assignmentId)
                .ToListAsync();

            ViewBag.AssignmentTitle = assignment.Title;
            return View(submissions);
        }

        [HttpGet]
        public async Task<IActionResult> GradeSubmission(int id)
        {
            var submission = await _context.AssignmentSubmissions
                .Include(s => s.Student)
                    .ThenInclude(s => s.User)
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null) return NotFound();

            return View(submission);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GradeSubmission(int id, [Bind("Id,Grade,Feedback")] AssignmentSubmission model)
        {
            // Kullanıcı ve öğretmen bilgisini ayrı ayrı al
            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            var submission = await _context.AssignmentSubmissions
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission?.Assignment?.Course?.TeacherId != teacher?.Id)
                return Forbid();

            submission.Grade = model.Grade;
            submission.Feedback = model.Feedback;
            submission.GradedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction("ViewSubmissions", new { assignmentId = submission.AssignmentId });
        }

        public async Task<IActionResult> MyCourses()
        {
            var teacherId = await GetCurrentTeacherId();
            var courses = await _context.Courses
                .Include(c => c.Package)
                .Include(c => c.CourseMaterials)
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();

            return View(courses);
        }

        // Kurs Yönetim Sayfası
        public async Task<IActionResult> ManageCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseMaterials)
                .Include(c => c.Assignments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null || course.TeacherId != await GetCurrentTeacherId())
                return NotFound();

            return View(course);
        }

        [HttpGet]
        public IActionResult UploadMaterial(int courseId)
        {
            // Kurs ID'sini view'a taşı
            ViewBag.CourseId = courseId;
            return View();
        }

        // Materyal Yükleme
        [HttpPost]
        public async Task<IActionResult> UploadMaterial(int courseId, IFormFile file)
        {
            var allowedExtensions = new[] { ".mp4", ".pdf", ".docx", ".pptx", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Geçersiz dosya formatı");

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            Directory.CreateDirectory(uploadPath);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var userId = _userManager.GetUserId(User);
            if (!int.TryParse(userId, out int uploadedById))
            {
                TempData["ErrorMessage"] = "Geçersiz kullanıcı kimliği!";
                return RedirectToAction("ManageCourse", new { id = courseId });
            }

            var material = new CourseMaterial
            {
                CourseId = courseId,
                FilePath = uniqueFileName,
                Type = GetFileType(extension),
                UploadDate = DateTime.Now,
                UploadedById = uploadedById // Artık int
            };

            _context.CourseMaterials.Add(material);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageCourse", new { id = courseId });
        }

        // Ödev Oluşturma
        [HttpPost]
        public async Task<IActionResult> CreateAssignment(
          [Bind("Title,Description,DueDate,CourseId")] Assignment assignment,
          IFormFile file)
        {
            try
            {
                // Özel validasyonlar
                if (assignment.DueDate < DateTime.Now)
                {
                    ModelState.AddModelError("DueDate", "Geçmiş tarih seçilemez");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    TempData["ErrorMessage"] = "Hatalar: " + string.Join(", ", errors);
                    return RedirectToAction("ManageCourse", new { id = assignment.CourseId });
                }

                // Dosya yükleme işlemleri
                if (file != null)
                {
                    var uploadPath = Path.Combine(_env.WebRootPath, "assignments");
                    Directory.CreateDirectory(uploadPath);

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadPath, uniqueFileName);

                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    assignment.AttachmentPath = uniqueFileName;
                }

                // Veritabanı işlemleri
                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ödev başarıyla oluşturuldu!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }

            return RedirectToAction("ManageCourse", new { id = assignment.CourseId });
        }

        private async Task<int> GetCurrentTeacherId()
        {
            var user = await _userManager.GetUserAsync(User);
            return _context.Teachers
                .FirstOrDefault(t => t.UserId == user.Id)?.Id ?? 0;
        }

        private string GetFileType(string extension)
        {
            return extension switch
            {
                ".mp4" => "Video",
                ".pdf" => "PDF",
                ".docx" => "Word",
                ".pptx" => "PowerPoint",
                _ => "Image"
            };
        }

        // TeachersController.cs
        public async Task<IActionResult> TeacherDesign()
        {
            var teachers = await _context.Teachers
                .Include(t => t.User)
                .ToListAsync();
            return View(teachers);
        }


        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Teachers.Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }


        // GET: Teachers/Create
        // GET: Teachers/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "Name");
            return View();
        }

        // POST: Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,UserId,Biography,ExperienceYears,Certifications")] Teacher teacher,
            int BranchId) // BranchId parametre olarak al
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();

                // TeacherBranch ilişkisini oluştur
                var teacherBranch = new TeacherBranch
                {
                    TeacherId = teacher.Id,
                    BranchId = BranchId
                };
                _context.TeacherBranches.Add(teacherBranch);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Hata durumunda listeleri tekrar yükle
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", teacher.UserId);
            ViewData["Branches"] = new SelectList(_context.Branches, "Id", "Name");
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", teacher.UserId);
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Biography,ExperienceYears,Certifications")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", teacher.UserId);
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
