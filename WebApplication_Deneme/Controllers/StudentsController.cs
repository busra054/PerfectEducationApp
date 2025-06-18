using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_Domain.Entities;
using WebApplication_Infrastructure.Data;

namespace WebApplication_Deneme.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;

        public StudentsController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: /Students/MyTeachers
        [HttpGet]
        [Authorize(Roles = "Öğrenci")]
        public async Task<IActionResult> MyTeachers()
        {

            var userId = _userManager.GetUserId(User);
            int parsedUserId = int.Parse(userId); // parse string to int

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Teacher)
                            .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(s => s.UserId == parsedUserId);


            if (student == null)
                return View(new List<MyTeacherViewModel>()); // boş liste


            var teachers = student.Enrollments
                .Select(e => e.Course.Teacher)
                .Distinct()
                .ToList();

            var vm = teachers.Select(t => new MyTeacherViewModel
            {
                TeacherId = t.Id,
                Name = t.User.Name,
                Biography = t.Biography,
                Courses = student.Enrollments
                                  .Where(e => e.Course.TeacherId == t.Id)
                                  .Select(e => e.Course)
                                  .ToList()
            })
            .ToList();

            return View(vm);
        }


        // GET: /Students/MyAppointments
        [HttpGet]
        [Authorize(Roles = "Öğrenci")]
        public async Task<IActionResult> MyAppointments()
        {

            var userId = _userManager.GetUserId(User);
            int parsedUserId = int.Parse(userId); // parse string to int

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == parsedUserId);

            if (student == null)
                return View(new List<Appointment>()); // Öğrenci kaydı yoksa boş liste dön


            var list = await _context.Appointments
                .Include(a => a.Package)
                .Include(a => a.Course)
                .Include(a => a.Teacher).ThenInclude(t => t.User)
                .Where(a => a.StudentId == student.Id)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return View(list);
        }

        // Öğrenci bir randevuya tıkladığında Zoom linkine yönlendirir
        public async Task<IActionResult> Join(int id)
        {
            var userId = int.Parse(_userManager.GetUserId(User));
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return Forbid();

            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null || appt.StudentId != student.Id)
                return Forbid();

            if (string.IsNullOrWhiteSpace(appt.ZoomLink))
            {
                TempData["Error"] = "Zoom linki henüz paylaşılmadı.";
                return RedirectToAction(nameof(MyAppointments));
            }

            // Harici URL’e yönlendir
            return Redirect(appt.ZoomLink);
        }


        [HttpGet]
        public async Task<IActionResult> ViewSubmission(int id)
        {
            var submission = await _context.AssignmentSubmissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null) return NotFound();

            // Null kontrolü ve int parse işlemi
            var currentUserIdString = _userManager.GetUserId(User);
            if (!int.TryParse(currentUserIdString, out int currentUserId))
            {
                return Forbid(); // Geçersiz kullanıcı ID formatı
            }

            if (submission.Student.UserId != currentUserId)
                return Forbid();

            return View(submission);
        }

        public async Task<IActionResult> CourseDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.CourseMaterials)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Assignments)
                            .ThenInclude(a => a.Submissions)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            var course = student?.Enrollments
                .Select(e => e.Course)
                .FirstOrDefault(c => c.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }

        [HttpGet]
        public async Task<IActionResult> SubmitAssignment(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                  .Include(a => a.Submissions) // Eklendi
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
              .FirstOrDefaultAsync(s => s.User.Id == user.Id);

            if (!await _context.CourseEnrollments
                .AnyAsync(ce => ce.StudentId == student.Id && ce.CourseId == assignment.CourseId))
                return Forbid();

            var submission = await _context.AssignmentSubmissions
                .FirstOrDefaultAsync(s => s.AssignmentId == id && s.StudentId == user.Id);

            return View(submission ?? new AssignmentSubmission { AssignmentId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAssignment(int id, AssignmentSubmission model, IFormFile file)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                  .Include(s => s.User)
                  .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (assignment == null || student == null)
            {
                ModelState.AddModelError("", "Geçersiz veri girişi");
                return View(model);
            }

            // Yeni gönderimde dosya kontrolü
            if (model.Id == 0 && file == null)
            {
                ModelState.AddModelError("", "Yeni ödev gönderimi için dosya yüklemek zorunludur!");
                return View(model);
            }

            string fileName = null;
            if (file != null)
            {
                // Dosya validasyonları
                var allowedExtensions = new[] { ".docx", ".pdf", ".txt", ".zip", ".rar", ".xlsx", ".mp4" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "Geçersiz dosya formatı!");
                    return View(model);
                }

                // Dosya yükleme
                var uploadsPath = Path.Combine(_env.WebRootPath, "submissions");
                fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                Directory.CreateDirectory(uploadsPath);
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var submission = await _context.AssignmentSubmissions
                .FirstOrDefaultAsync(s => s.Id == model.Id);

            if (submission == null)
            {
                // Yeni gönderim
                submission = new AssignmentSubmission
                {
                    AssignmentId = id,
                    StudentId = student.Id,
                    FilePath = fileName,
                    Comments = model.Comments,
                    SubmissionDate = DateTime.Now // Tarih ataması eklendi
                };
                _context.Add(submission);
            }
            else
            {
                // Güncelleme
                submission.Comments = model.Comments;
                submission.SubmissionDate = DateTime.Now;
                if (file != null)
                {
                    submission.FilePath = fileName; // Yeni dosya
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("CourseDetails", new { id = assignment.CourseId });
        }


        [HttpGet]
        [Authorize(Roles = "Öğrenci")]
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index", "Account");

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Package)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Teacher)
                            .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
                return View(new List<CourseEnrollment>()); // TİP UYUMLU hale getirildi ✅


            var enrollments = student.Enrollments.ToList();
            if (!enrollments.Any())
                ViewBag.Message = "Henüz hiç kursa kaydınız bulunmamaktadır.";

            return View(enrollments);
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Students.Include(s => s.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,EnrollmentDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", student.UserId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", student.UserId);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,EnrollmentDate")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", student.UserId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
