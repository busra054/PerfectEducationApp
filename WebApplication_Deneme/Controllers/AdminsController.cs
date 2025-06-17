using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WebApplication_Infrastructure.Data;
using WebApplication_Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace WebApplication_Deneme.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

            var adminEntities = adminUsers.Select(u => new Admin
            {
                // AdminId kaldırıldı, Id kullanılıyor
                Id = u.Id,
                AdminName = u.Name,
                AdminPassword = "••••••••"
            });

            var viewModel = new AdminDashboardViewModel
            {
                Admins = adminEntities,
                PendingRequests = await _context.TeacherRequests
                    .Include(r => r.User)
                    .Where(r => r.Status == RequestStatus.Pending)
                    .ToListAsync()
            };

            return View(viewModel);
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
                var teacher = new Teacher
                {
                    UserId = request.UserId,
                    Biography = request.Biography,
                    ExperienceYears = request.ExperienceYears,
                    Certifications = request.CertificationsPath
                };

                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();

                if (request.BranchId > 0)
                {
                    _context.TeacherBranches.Add(new TeacherBranch
                    {
                        TeacherId = teacher.Id,
                        BranchId = request.BranchId
                    });
                }

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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // AdminId yerine Id kullanıldı
            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.Id == id);

            if (admin == null) return NotFound();

            return View(admin);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name, string Email, string password)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ErrorMessage"] = "Tüm alanları doldurmalısınız.";
                return View();
            }

            var user = new User
            {
                UserName = Email,
                Email = Email,
                Name = Name,
                Role = "Admin"
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["SuccessMessage"] = "Yeni admin başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // AdminId yerine Id kullanıldı
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdminName,AdminPassword")] Admin admin)
        {
            if (id != admin.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            // AdminId yerine Id kullanıldı
            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.Id == id);

            if (admin == null) return NotFound();

            return View(admin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }
    }
}
