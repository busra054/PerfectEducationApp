using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_Infrastructure.Data;
using WebApplication_Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication_Deneme.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;   


        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        // GET: /Account/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Account/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View(model);
            }

            // 1) Normal sign-in denemesi
            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Geçersiz giriş denemesi.");
                return View(model);
            }

            // 2) Eğer bekleyen bir Öğretmen başvurusu varsa girişe izin verme
            var pending = await _context.TeacherRequests
                                .Where(r => r.UserId == user.Id && r.Status == RequestStatus.Pending)
                                .AnyAsync();
            if (pending)
            {
                // Oturumu sonlandır ve uyarı göster
                await _signInManager.SignOutAsync();
                ModelState.AddModelError("", "Öğretmen başvurunuz henüz onaylanmadı.");
                return View(model);
            }

            // 3) Rollere göre yönlendir
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "Admins");
            if (roles.Contains("Öğrenci"))
                return RedirectToAction("MyCourses", "Students");
            if (roles.Contains("Öğretmen"))
                return RedirectToAction("MyCourses", "Teachers");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}