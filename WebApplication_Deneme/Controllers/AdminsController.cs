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
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApplication_Deneme.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminCookies", Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> PendingTeachers()
        {
            var requests = await _context.TeacherRequests
                .Include(r => r.User)
                .Include(r => r.Branch)
                .Where(r => r.Status == RequestStatus.Pending)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> ApprovedTeachers()
        {
            var requests = await _context.TeacherRequests
                .Include(r => r.User)
                .Where(r => r.Status == RequestStatus.Approved)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> RejectedTeachers()
        {
            var requests = await _context.TeacherRequests
                .Include(r => r.User)
                .Where(r => r.Status == RequestStatus.Rejected)
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveTeacher(int requestId)
        {
            var request = await _context.TeacherRequests
                .Include(r => r.User)
                .Include(r => r.Branch)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request != null)
            {
                // Öğretmen oluştur
                var teacher = new Teacher
                {
                    UserId = request.UserId,
                    Biography = request.Biography,
                    ExperienceYears = request.ExperienceYears,
                    Certifications = request.CertificationsPath
                };

                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();

                // Branş ilişkisini ekle
                if (request.BranchId > 0) // int için doğru kontrol
                {
                    _context.TeacherBranches.Add(new TeacherBranch
                    {
                        TeacherId = teacher.Id,
                        BranchId = request.BranchId // Direkt değeri kullan
                    });
                }

                // Başvuru durumunu güncelle
                request.Status = RequestStatus.Approved;
                request.ReviewedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PendingTeachers");
        }

        [HttpPost]
        public async Task<IActionResult> RejectTeacher(int requestId, string adminNotes)
        {
            var request = await _context.TeacherRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request != null)
            {
                request.Status = RequestStatus.Rejected;
                request.ReviewedDate = DateTime.UtcNow;
                request.AdminNotes = adminNotes;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PendingTeachers");
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.AdminName == model.AdminName &&
                                             a.AdminPassword == model.AdminPassword);

                if (admin != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.AdminName),
                new Claim(ClaimTypes.Role, "Admin")
            };

                    var claimsIdentity = new ClaimsIdentity(claims, "AdminCookies"); // Scheme ile eşleşmeli
                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                    // AdminCookies scheme'ini kullanarak giriş yap
                    await HttpContext.SignInAsync(
                        "AdminCookies",
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Admins");
                }

                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
            }
            return View(model);
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Admins");
            }

            var viewModel = new AdminDashboardViewModel
            {
                Admins = await _context.Admins.ToListAsync(),
                PendingRequests = await _context.TeacherRequests
                    .Include(r => r.User)
                    .Where(r => r.Status == RequestStatus.Pending)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,AdminName,AdminPassword")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,AdminName,AdminPassword")] Admin admin)
        {
            if (id != admin.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminId))
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
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminId == id);
        }
    }
}
