using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication_Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApplication_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication_Deneme.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            Console.WriteLine($"Kullanýcý Rolü: {user.Role}"); // Debug için

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            // Yönlendirme yapma, doðrudan View döndür
            return View();
        }

        private async Task<IActionResult> HandleTeacherRedirect(int userId)
        {
            var latestRequest = await _context.TeacherRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestDate)
                .FirstOrDefaultAsync();

            if (latestRequest == null)
            {
                // Baþvuru yapmamýþ öðretmen
                return RedirectToAction("Apply", "TeachersRequests");
            }

            return latestRequest.Status switch
            {
                RequestStatus.Approved => RedirectToAction("Index", "TeachersRequests"),
                RequestStatus.Pending => RedirectToAction("PendingApproval", "TeachersRequests"),
                RequestStatus.Rejected => RedirectToAction("Rejected", "TeachersRequests", new
                {
                    message = latestRequest.AdminNotes
                }),
                _ => RedirectToAction("Error", "Home")
            };
        }

        public IActionResult PendingApproval()
        {
            return View();
        }

        public IActionResult Rejected(string message)
        {
            ViewBag.RejectionMessage = message;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}