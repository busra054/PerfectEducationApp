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

namespace WebApplication_Deneme.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;


        public UsersController(
            ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Biography = user.Biography,
                ExistingCertificationPath = user.Certifications,
                ExistingProfileImagePath = user.ProfileImagePath
            };
            return View(model);
        }


        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Role")] User user, string password)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları doldurun";
                return View(user);
            }

            // 1) Kullanıcı oluştur
            var newUser = new User
            {
                UserName = user.Email,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role
            };
            var result = await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                TempData["ErrorMessage"] = "Kullanıcı oluşturulamadı.";
                return View(user);
            }

            // 2) Rol atama & TeacherRequest
            if (user.Role == "Öğretmen")
            {
                return RedirectToAction("Apply", "TeacherRequests", new { userId = newUser.Id });

            }
            else
            {
                // Diğer roller (örn. Öğrenci)
                await _userManager.AddToRoleAsync(newUser, user.Role);
                TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu!";
                return RedirectToAction("Index", "Account");
            }
        }



        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Role")] User user, string? newPassword)
        {
            if (id != user.Id) return NotFound();

            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (existingUser == null) return NotFound();

            // Bilgileri güncelle
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;

            // Şifre değişikliği
            if (!string.IsNullOrEmpty(newPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                await _userManager.ResetPasswordAsync(existingUser, token, newPassword);
            }

            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(user);
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
