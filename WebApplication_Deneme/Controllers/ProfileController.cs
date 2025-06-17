using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_Domain.Entities;

namespace WebApplication_Deneme.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(UserManager<User> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.Users
                .Include(u => u.TeacherProfile)
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (user == null) return NotFound();

            var vm = new UserProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Biography = user.TeacherProfile?.Biography, // sadece öğretmen için
                ExistingCertificationPath = user.TeacherProfile?.Certifications, // sadece öğretmen için
                ExistingProfileImagePath = user.ProfileImagePath,
                InstagramUrl = user.InstagramUrl,
                TwitterUrl = user.TwitterUrl,
                FacebookUrl = user.FacebookUrl
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.Users
                .Include(u => u.TeacherProfile)
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync(u => u.Id == model.Id);

            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Email = model.Email;

            // Sadece öğretmense biyografi güncelle
            if (user.TeacherProfile != null)
            {
                user.TeacherProfile.Biography = model.Biography;

                // Sertifika yükleme sadece öğretmen için
                if (model.CertificationFile != null)
                {
                    var certFolder = Path.Combine(_env.WebRootPath, "files/certifications");
                    Directory.CreateDirectory(certFolder);
                    var certName = $"cert_{Guid.NewGuid()}{Path.GetExtension(model.CertificationFile.FileName)}";
                    var certPath = Path.Combine(certFolder, certName);
                    using var certStream = new FileStream(certPath, FileMode.Create);
                    await model.CertificationFile.CopyToAsync(certStream);
                    user.TeacherProfile.Certifications = $"/files/certifications/{certName}";
                }
            }

            // Sosyal medya
            user.InstagramUrl = model.InstagramUrl;
            user.TwitterUrl = model.TwitterUrl;
            user.FacebookUrl = model.FacebookUrl;

            // Profil resmi
            if (model.ProfileImage != null)
            {
                var imgFolder = Path.Combine(_env.WebRootPath, "img/profiles");
                Directory.CreateDirectory(imgFolder);
                var imgName = $"prof_{Guid.NewGuid()}{Path.GetExtension(model.ProfileImage.FileName)}";
                var imgPath = Path.Combine(imgFolder, imgName);
                using var imgStream = new FileStream(imgPath, FileMode.Create);
                await model.ProfileImage.CopyToAsync(imgStream);
                user.ProfileImagePath = $"/img/profiles/{imgName}";
            }

            // Şifre değişikliği
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                return View(model);
            }

            TempData["Success"] = "Profiliniz başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
