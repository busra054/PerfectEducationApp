using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_Infrastructure.Data;
using WebApplication_Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

namespace WebApplication_Deneme.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;


        public CoursesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Öğrenci"))
            {
                var user = await _userManager.GetUserAsync(User);
                var student = await _context.Students
                    .Include(s => s.Enrollments)
                        .ThenInclude(e => e.Course)
                            .ThenInclude(c => c.Teacher)
                    .FirstOrDefaultAsync(s => s.UserId == user.Id);

                var courses = student?.Enrollments
                    .Select(e => e.Course)
                    .ToList() ?? new List<Course>();

                return View(courses);
            }
            else
            {
                var courses = await _context.Courses
                    .Include(c => c.Teacher)
                    .Include(c => c.Package)
                    .ToListAsync();

                return View(courses);
            }
        }
        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Öğretmen,Admin")]
        public IActionResult Create()
        {
            // Paketlerin veritabanından çekildiğinden emin olun
            var packages = _context.Packages.ToList();
            ViewBag.Packages = new SelectList(packages, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Öğretmen,Admin")]
        public async Task<IActionResult> Create([Bind("Name,Description,PackageId")] Course course)
        {
            // Paket listesini yeniden yükle
            ViewBag.Packages = new SelectList(await _context.Packages.ToListAsync(), "Id", "Name");

            // 1. Öğretmen ID'sini al
            var teacherId = await GetCurrentTeacherId();
            if (teacherId == 0)
            {
                ModelState.AddModelError("", "Öğretmen profiliniz bulunamadı!");
                return View(course);
            }
            course.TeacherId = teacherId;

            // 2. Paket varlığını kontrol et
            var package = await _context.Packages.FindAsync(course.PackageId);
            if (package == null)
            {
                ModelState.AddModelError("PackageId", "Geçersiz paket seçimi!");
                return View(course);
            }

            // 3. İlişkileri manuel olarak ata
            course.Teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            course.Package = package;

            // 4. ModelState kontrolü
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(course);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"{course.Name} başarıyla oluşturuldu!";
                    return RedirectToAction("MyCourses", "Teachers");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Veritabanı hatası: " + ex.InnerException?.Message);
                }
            }

            // Hata durumunda view'a dön
            return View(course);
        }

        private async Task<int> GetCurrentTeacherId()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return 0;

            // User.Id (int) ile Teacher.UserId karşılaştırması
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            return teacher?.Id ?? 0;
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", course.TeacherId);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,TeacherId")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", course.TeacherId);
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            if (course != null)
            {
                _context.Course.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}
