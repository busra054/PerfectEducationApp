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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WebApplication_Deneme.Controllers
{
    [Authorize(Roles = "Öğretmen,Öğrenci,Admin")]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> GetNewMessages()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new List<object>());

            var messages = await _context.Messages
                .Where(m => m.ReceiverId == user.Id && !m.IsDeletedByReceiver && !m.IsRead)
                .Include(m => m.Sender)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            var result = messages.Select(m => new
            {
                senderId = m.SenderId,
                senderName = m.Sender.Name,
                senderProfileImage = m.Sender.ProfileImagePath,
                messageText = m.MessageText,
                sentDate = m.SentDate.ToString("g")
            });

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(0);

            var count = await _context.Messages
                .CountAsync(m => m.ReceiverId == user.Id && !m.IsDeletedByReceiver && !m.IsRead);

            return Json(count);
        }

        // MarkMessagesAsRead metodunu güncelle
        [HttpPost]
        public async Task<IActionResult> MarkMessagesAsRead(int otherUserId)
        {
            var meId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var unreadMessages = await _context.Messages
                .Where(m => m.SenderId == otherUserId && m.ReceiverId == meId && !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Count > 0)
            {
                unreadMessages.ForEach(m => m.IsRead = true);
                await _context.SaveChangesAsync();
            }

            // Yeni eklenen kod: Tüm okunmamış mesaj sayısını güncelle
            var unreadCount = await _context.Messages
                .CountAsync(m => m.ReceiverId == meId && !m.IsDeletedByReceiver && !m.IsRead);

            return Json(new { success = true, unreadCount });
        }


        // GET: Chat
        public async Task<IActionResult> Chat(int otherUserId)
        {
            var me = await _userManager.GetUserAsync(User);
            var other = await _context.Users.FindAsync(otherUserId);
            if (other == null) return NotFound();

            ViewBag.OtherUserId = otherUserId;
            ViewBag.OtherUserName = other.Name;

            var msgs = await _context.Messages
                .Where(m =>
                    ((m.SenderId == me.Id && m.ReceiverId == otherUserId) ||
                     (m.SenderId == otherUserId && m.ReceiverId == me.Id)) &&
                    !(m.SenderId == me.Id && m.IsDeletedBySender) &&
                    !(m.ReceiverId == me.Id && m.IsDeletedByReceiver)
                )
                .OrderBy(m => m.SentDate)
                .ToListAsync();

            return View(msgs);
        }

        // GET: Messages
        // Sohbet partnerleri listeler
        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var me = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(me);
            if (roles.Contains("Admin"))
                return Forbid();

            var allowed = new HashSet<User>();

            if (roles.Contains("Öğrenci"))
            {
                var myStudent = await _context.Students
                    .Include(s => s.Enrollments).ThenInclude(e => e.Course)
                    .FirstOrDefaultAsync(s => s.UserId == me.Id);

                var courseIds = myStudent?.Enrollments.Select(e => e.CourseId).ToList()
                                ?? new List<int>();

                var peers = await _context.CourseEnrollments
                    .Where(e => courseIds.Contains(e.CourseId) && e.Student.UserId != me.Id)
                    .Select(e => e.Student.User)
                    .Distinct()
                    .ToListAsync();

                var teachers = await _context.Courses
                    .Where(c => courseIds.Contains(c.Id))
                    .Select(c => c.Teacher.User)
                    .Distinct()
                    .ToListAsync();

                foreach (var user in peers)
                    allowed.Add(user);
                foreach (var user in teachers)
                    allowed.Add(user);
            }
            else if (roles.Contains("Öğretmen"))
            {
                var myTeacher = await _context.Teachers
                    .Include(t => t.Courses)
                      .ThenInclude(c => c.EnrolledStudents)
                        .ThenInclude(e => e.Student).ThenInclude(s => s.User)
                    .FirstOrDefaultAsync(t => t.UserId == me.Id);

                var studentUsers = myTeacher?.Courses
                    .SelectMany(c => c.EnrolledStudents.Select(e => e.Student.User))
                    .Distinct() ?? Enumerable.Empty<User>();

                var allTeachers = await _userManager.GetUsersInRoleAsync("Öğretmen");
                var peers = allTeachers.Where(u => u.Id != me.Id);

                foreach (var user in studentUsers)
                    allowed.Add(user);
                foreach (var user in peers)
                    allowed.Add(user);
            }

            ViewBag.AvailableUsers = new SelectList(
                allowed.OrderBy(u => u.Name).ToList(),
                "Id", "Name"
            );

            var conversations = _context.Messages
                .Where(m => m.SenderId == me.Id || m.ReceiverId == me.Id)
                .AsEnumerable()   // IQueryable → IEnumerable
                .Where(m =>
                    !(m.SenderId == me.Id && m.IsDeletedBySender) &&
                    !(m.ReceiverId == me.Id && m.IsDeletedByReceiver))
                .GroupBy(m => m.SenderId == me.Id ? m.ReceiverId : m.SenderId)
                .Select(g => new ConversationViewModel
                {
                    Partner = _context.Users.Find(g.Key)!,
                    LastMessage = g.OrderByDescending(x => x.SentDate).First()
                })
                .OrderByDescending(c => c.LastMessage.SentDate)
                .ToList();

            return View(conversations);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConversation(int otherUserId)
        {
            var me = await _userManager.GetUserAsync(User);

            var messages = await _context.Messages
                .Where(m => (m.SenderId == me.Id && m.ReceiverId == otherUserId)
                         || (m.SenderId == otherUserId && m.ReceiverId == me.Id))
                .ToListAsync();

            foreach (var msg in messages)
            {
                if (msg.SenderId == me.Id)
                    msg.IsDeletedBySender = true;
                else if (msg.ReceiverId == me.Id)
                    msg.IsDeletedByReceiver = true;

                // Eğer iki taraf da silmişse mesajı tamamen kaldır
                if (msg.IsDeletedBySender && msg.IsDeletedByReceiver)
                    _context.Messages.Remove(msg);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // POST Messages/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            var msg = await _context.Messages.FindAsync(id);
            if (msg == null || msg.SenderId != me.Id) return Forbid();
            _context.Messages.Remove(msg);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Chat), new { otherUserId = msg.SenderId == me.Id ? msg.ReceiverId : msg.SenderId });
        }



        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SenderId,ReceiverId,MessageText,SentDate")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "Id", message.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", message.SenderId);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var message = await _context.Messages.FindAsync(id);
            if (message == null) return NotFound();

            ViewBag.SenderList = new SelectList(_context.Users, "Id", "UserName", message.SenderId);
            ViewBag.ReceiverList = new SelectList(_context.Users, "Id", "UserName", message.ReceiverId);

            return View(message);
        }

        // POST: Messages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string messageText)
        {
            if (id == 0 || string.IsNullOrEmpty(messageText))
                return BadRequest();

            var existingMessage = await _context.Messages.FindAsync(id);
            if (existingMessage == null)
                return NotFound();

            existingMessage.MessageText = messageText;
            existingMessage.SentDate = DateTime.Now;

            _context.Update(existingMessage);
            await _context.SaveChangesAsync();

            return Json(new { success = true, messageText = existingMessage.MessageText });
        }


        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            var msg = await _context.Messages.FindAsync(id);
            if (msg == null || msg.SenderId != me.Id)
                return Forbid();

            var otherId = msg.SenderId == me.Id ? msg.ReceiverId : msg.SenderId;
            _context.Messages.Remove(msg);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Chat), new { otherUserId = otherId });
        }


        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
