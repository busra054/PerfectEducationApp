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

namespace WebApplication_Deneme.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public PaymentsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Payments/Purchase/5 (Paket seçim formunu göster)
        [HttpGet]
        public async Task<IActionResult> Purchase(int? packageId)
        {
            if (packageId == null) return NotFound();

            var package = await _context.Packages.FindAsync(packageId);
            if (package == null) return NotFound();

            // Yeni bir Payment modeli oluştur ve paket bilgilerini ata
            var payment = new Payment
            {
                PackageId = package.Id,
                Amount = package.Price,
                PaymentDate = DateTime.Now
            };

            // Paket bilgilerini ViewBag'e ekle (view'da gösterim için)
            ViewBag.Package = package;

            return View(payment); // Modeli view'a gönder
        }

        // POST: Payments/Purchase/5 (Ödemeyi işle)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Purchase(int packageId, [Bind("CardNumber,ExpiryMonth,ExpiryYear,CVC,CardHolderName")] Payment payment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Giriş yapmamışsa yönlendir
            }

            var package = await _context.Packages
                .Include(p => p.Courses)
                .FirstOrDefaultAsync(p => p.Id == packageId);
            if (package == null)
            {
                return NotFound();
            }

            // Öğrenci kontrolü ve otomatik oluşturma
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (student == null)
            {
                student = new Student { UserId = user.Id };
                _context.Students.Add(student);
                await _context.SaveChangesAsync(); // Önce student'i kaydet
            }

            payment.StudentId = student.Id;
            payment.PackageId = packageId;
            payment.Amount = package.Price;
            payment.PaymentDate = DateTime.Now;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Kurs kayıt işlemleri
                foreach (var course in package.Courses)
                {
                    if (!_context.CourseEnrollments.Any(ce => ce.StudentId == student.Id && ce.CourseId == course.Id))
                    {
                        _context.CourseEnrollments.Add(new CourseEnrollment
                        {
                            StudentId = student.Id,
                            CourseId = course.Id,
                            EnrollmentDate = DateTime.Now
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("MyCourses", "Students");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Hata: " + ex.Message);
                ViewBag.Package = package;
                return View(payment);
            }
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var payments = await _context.Payments
                .Include(p => p.Package)
                .Include(p => p.Student)
                    .ThenInclude(s => s.User) // Öğrenci üzerinden user
                .ToListAsync();
            return View(payments);
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var payment = await _context.Payments
                .Include(p => p.Package)
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create (Admin için manuel ödeme ekleme)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["PackageId"] = new SelectList(_context.Packages, "Id", "Name");
            ViewData["StudentId"] = new SelectList(
                _context.Students.Include(s => s.User),
                "Id",
                "User.Name" // Öğrenci adlarını göster
            );
            return View();
        }


        // POST: Payments/Create (Admin için)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,PackageId,Amount")] Payment payment)
        {
            payment.PaymentDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PackageId"] = new SelectList(_context.Packages, "Id", "Name", payment.PackageId);
            ViewData["StudentId"] = new SelectList(_context.Students.Include(s => s.User), "Id", "User.Name", payment.StudentId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            ViewData["PackageId"] = new SelectList(_context.Packages, "Id", "Name", payment.PackageId);
            ViewData["StudentId"] = new SelectList(
                _context.Students.Include(s => s.User),
                "Id",
                "User.Name",
                payment.StudentId
            );

            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,PackageId,PaymentDate,Amount")] Payment payment)
        {
            if (id != payment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["PackageId"] = new SelectList(_context.Packages, "Id", "Name", payment.PackageId);
            ViewData["StudentId"] = new SelectList(_context.Students.Include(s => s.User), "Id", "User.Name", payment.StudentId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Package)
                .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
