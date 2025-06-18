using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_Domain.Entities;
using WebApplication_Infrastructure.Data;

namespace WebApplication_Deneme.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AssignmentsController(
            ApplicationDbContext context,
            IWebHostEnvironment env) 
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = "Admin,Öğretmen,Öğrenci")]
        public async Task<IActionResult> Index()
        {
            // 1) Claim’ten gelen userId string
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2) String’i int’e çeviriyoruz
            if (!int.TryParse(userIdString, out var userId))
            {
                // Parse edilemediyse yetkisiz gibi davranabilirsiniz
                return Forbid();
            }

            // 3) Sorguyu hazırla (Course → Teacher ilişkisiyle birlikte)
            IQueryable<Assignment> query = _context.Assignments
                .Include(a => a.Course)
                    .ThenInclude(c => c.Teacher);

            if (User.IsInRole("Öğretmen"))
            {
                // Teacher nav‑prop’unu include edip, int userId ile karşılaştıralım
                var teacher = await _context.Teachers
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.User.Id == userId);
                if (teacher == null)
                    return Forbid();

                query = query.Where(a => a.Course.TeacherId == teacher.Id);
            }
            else if (User.IsInRole("Öğrenci"))
            {
                var student = await _context.Students
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.User.Id == userId);
                if (student == null)
                    return Forbid();

                query = query.Where(a =>
                    _context.CourseEnrollments
                        .Any(e => e.CourseId == a.CourseId && e.StudentId == student.Id)
                );
            }
            // Admin: filtre yok

            var assignments = await query
                .OrderBy(a => a.DueDate)
                .ToListAsync();

            return View(assignments);
        }


        // GET: Assignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // GET: Assignments/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description");
            return View();
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssignment(
            [Bind("Title,Description,DueDate,CourseId")] Assignment assignment,
            IFormFile file)
        {
            try
            {
                // ModelState kontrolü
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Hata: {error.ErrorMessage}");
                    }
                    TempData["ErrorMessage"] = "Geçersiz veri girişi! Lütfen tüm zorunlu alanları doldurun.";
                    return RedirectToAction("ManageCourse", new { id = assignment.CourseId });
                }

                // Dosya yükleme
                if (file != null)
                {
                    var uploadPath = Path.Combine(_env.WebRootPath, "assignments");
                    Directory.CreateDirectory(uploadPath);

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadPath, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
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
                Console.WriteLine($"Hata: {ex.Message}");
                TempData["ErrorMessage"] = "Ödev oluşturulurken bir hata meydana geldi!";
            }

            return RedirectToAction("ManageCourse", new { id = assignment.CourseId });
        }

        // GET: Assignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", assignment.CourseId);
            return View(assignment);
        }

        // POST: Assignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,AttachmentPath,CreatedAt,CourseId")] Assignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", assignment.CourseId);
            return View(assignment);
        }

        // GET: Assignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }
    }
}
