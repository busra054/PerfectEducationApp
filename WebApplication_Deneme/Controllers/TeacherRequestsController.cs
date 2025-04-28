using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_Domain.Entities;
using WebApplication_Infrastructure.Data;

namespace WebApplication_Deneme.Controllers
{
    public class TeacherRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;


        public TeacherRequestsController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<User> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        [HttpGet]
        public async Task<IActionResult> CheckStatus(int userId)
        {
            // Kullanıcıyı kontrol et
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı!";
                return RedirectToAction("Create", "Users");
            }

            // Başvuruyu ilişkili verilerle getir
            var request = await _context.TeacherRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (request == null)
            {
                TempData["Message"] = "Henüz başvurunuz bulunmamaktadır.";
                return RedirectToAction("Apply", new { userId });
            }

            return View(request);
        }

        public async Task<IActionResult> Apply(int userId)
        {
            var existingRequest = await _context.TeacherRequests
                .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == RequestStatus.Pending);

            if (existingRequest != null)
            {
                return RedirectToAction("CheckStatus", new { userId });
            }

            // Branşları çekerken Id > 0 kontrolü ekleyin
            var branches = await _context.Branches.Where(b => b.Id > 0).ToListAsync();

            if (!branches.Any())
            {
                TempData["ErrorMessage"] = "Sistemde kayıtlı branş bulunamadı!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Branches = new SelectList(branches, "Id", "Name");
            return View(new TeacherRequest { UserId = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int userId,
            [Bind("UserId,Biography,ExperienceYears,BranchId")] TeacherRequest request,
            IFormFile certificationFile)
        {
            if (!ModelState.IsValid)
            {
                var branches = await _context.Branches.ToListAsync();
                ViewBag.Branches = new SelectList(branches, "Id", "Name", request.BranchId);
                return View(request);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı!";
                    return RedirectToAction("Create", "Users");
                }

                if (certificationFile == null)
                {
                    ModelState.AddModelError("CertificationFile", "Sertifika dosyası gereklidir!");
                    ViewBag.Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
                    return View(request);
                }

                if (Path.GetExtension(certificationFile.FileName).ToLower() != ".pdf")
                {
                    ModelState.AddModelError("CertificationFile", "Sadece PDF dosyaları kabul edilir!");
                    ViewBag.Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
                    return View(request);
                }

                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "certificates");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(certificationFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await certificationFile.CopyToAsync(stream);
                }

                var teacherRequest = new TeacherRequest
                {
                    UserId = userId,
                    Biography = request.Biography,
                    ExperienceYears = request.ExperienceYears,
                    CertificationsPath = $"/uploads/certificates/{uniqueFileName}",
                    BranchId = request.BranchId, // Nullable olduğu için sorun yok
                    Status = RequestStatus.Pending,
                    RequestDate = DateTime.UtcNow
                };

                _context.TeacherRequests.Add(teacherRequest);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Başvurunuz başarıyla alındı!";
                return RedirectToAction("PendingApproval", new { userId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
                return View(request);
            }
        }

        public async Task<IActionResult> PendingApproval(int userId)
        {
            var request = await _context.TeacherRequests
                                        .FirstOrDefaultAsync(r => r.UserId == userId);

            if (request == null)
            {
                TempData["ErrorMessage"] = "Henüz başvurunuz bulunmamaktadır.";
                return RedirectToAction("Apply", new { userId });
            }

            return View(request);
        }


        // GET: TeacherRequests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TeacherRequests.Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TeacherRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherRequest = await _context.TeacherRequests
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacherRequest == null)
            {
                return NotFound();
            }

            return View(teacherRequest);
        }

        // GET: TeacherRequests/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: TeacherRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Biography,ExperienceYears,CertificationsPath,RequestDate,IsApproved")] TeacherRequest teacherRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacherRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", teacherRequest.UserId);
            return View(teacherRequest);
        }

        // GET: TeacherRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherRequest = await _context.TeacherRequests.FindAsync(id);
            if (teacherRequest == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", teacherRequest.UserId);
            return View(teacherRequest);
        }

        // POST: TeacherRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Biography,ExperienceYears,CertificationsPath,RequestDate,IsApproved")] TeacherRequest teacherRequest)
        {
            if (id != teacherRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacherRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherRequestExists(teacherRequest.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", teacherRequest.UserId);
            return View(teacherRequest);
        }

        // GET: TeacherRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherRequest = await _context.TeacherRequests
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacherRequest == null)
            {
                return NotFound();
            }

            return View(teacherRequest);
        }

        // POST: TeacherRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacherRequest = await _context.TeacherRequests.FindAsync(id);
            if (teacherRequest != null)
            {
                _context.TeacherRequests.Remove(teacherRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherRequestExists(int id)
        {
            return _context.TeacherRequests.Any(e => e.Id == id);
        }
    }
}
