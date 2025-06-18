using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_Infrastructure.Data;
using WebApplication_Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication_Deneme.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        [Authorize(Roles = "Öğretmen,Öğrenci,Admin")]
        public async Task<IActionResult> Index()
        {
            var list = await _context.Appointments
                .Include(a => a.Student).ThenInclude(s => s.User)
                .Include(a => a.Teacher).ThenInclude(t => t.User)
                .Include(a => a.Package)
                .Include(a => a.Course)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return View(list);
        }

        // GET: Appointments/Details/5
        [Authorize(Roles = "Öğretmen,Öğrenci,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appt = await _context.Appointments
                .Include(a => a.Student).ThenInclude(s => s.User)
                .Include(a => a.Teacher).ThenInclude(t => t.User)
                .Include(a => a.Package)
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appt == null) return NotFound();
            return View(appt);
        }


        // GET: Appointments/Create
        [Authorize(Roles = "Öğretmen")]
        public async Task<IActionResult> Create()
        {
            var teacherId = /* logic to retrieve teacherId from User.Identity */ 0;
            // Prepare packages list
            var packages = await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.Package)
                .Distinct()
                .ToListAsync();
            ViewBag.Packages = new SelectList(packages, "Id", "Name");
            ViewBag.Courses = new SelectList(Enumerable.Empty<Course>(), "Id", "Name");
            ViewBag.Students = new SelectList(Enumerable.Empty<Student>(), "Id", "User.Name");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Öğretmen")]
        public async Task<IActionResult> Create(int PackageId, int CourseId, int StudentId, DateTime Date, string Type)
        {
            var teacherId = /* logic to retrieve teacherId from User.Identity */ 0;
            if (CourseId == 0 || StudentId == 0 || Date <= DateTime.Now)
            {
                ModelState.AddModelError("", "Lütfen geçerli bir tarih ve seçimler yapın.");
            }
            if (!ModelState.IsValid)
            {
                // reload dropdowns
                var packages = await _context.Courses
                    .Where(c => c.TeacherId == teacherId)
                    .Select(c => c.Package)
                    .Distinct()
                    .ToListAsync();
                ViewBag.Packages = new SelectList(packages, "Id", "Name", PackageId);
                ViewBag.Courses = new SelectList(
                    _context.Courses.Where(c => c.PackageId == PackageId && c.TeacherId == teacherId),
                    "Id", "Name", CourseId);
                ViewBag.Students = new SelectList(
                    _context.CourseEnrollments.Where(e => e.CourseId == CourseId).Select(e => new { e.Student.Id, Name = e.Student.User.Name }),
                    "Id", "Name", StudentId);
                return View();
            }

            var appointment = new Appointment
            {
                PackageId = PackageId,
                CourseId = CourseId,
                StudentId = StudentId,
                TeacherId = teacherId,
                Date = Date,
                Type = Type
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Appointments/Edit/5
        [Authorize(Roles = "Öğretmen")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var appt = await _context.Appointments
                .Include(a => a.Package)
                .Include(a => a.Course)
                .Include(a => a.Student).ThenInclude(s => s.User)
                .Include(a => a.Teacher).ThenInclude(t => t.User)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (appt == null) return NotFound();
            ViewBag.CanEditZoom = appt.Date <= DateTime.Now;
            return View(appt);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Öğretmen")]
        public async Task<IActionResult> Edit(int id, string zoomLink)
        {
            // Rota parametresi ile gelen id kontrolü
            var appt = await _context.Appointments.FindAsync(id);
            if (appt == null)
                return NotFound();

            // ZoomLink’i güncelle
            appt.ZoomLink = zoomLink;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Delete/5
        [Authorize(Roles = "Öğretmen,Öğrenci,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Package)
                .Include(a => a.Course)
                .Include(a => a.Student)
                    .ThenInclude(s => s.User)
                .Include(a => a.Teacher)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
