using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_Domain.Entities;
using WebApplication_Infrastructure.Data;

namespace WebApplication_Deneme.Controllers
{
    public class AssignmentSubmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;


        public AssignmentSubmissionsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AssignmentSubmissions
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            // Declare the query as IQueryable<AssignmentSubmission>
            IQueryable<AssignmentSubmission> query = _context.AssignmentSubmissions
                .Include(a => a.Assignment)
                .Include(a => a.Student)
                    .ThenInclude(s => s.User);

            var applicationDbContext = _context.AssignmentSubmissions
                .Include(a => a.Assignment)
                .Include(a => a.Student)
                    .ThenInclude(s => s.User);

            if (User.IsInRole("Öğrenci"))
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (student != null)
                {
                    // Apply the Where clause to the query
                    query = query.Where(s => s.StudentId == student.Id);
                }
            }
            var submissions = await query.ToListAsync();
            return View(submissions);
        }

        // GET: AssignmentSubmissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentSubmission = await _context.AssignmentSubmissions
                .Include(a => a.Assignment)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignmentSubmission == null)
            {
                return NotFound();
            }

            return View(assignmentSubmission);
        }

        // GET: AssignmentSubmissions/Create
        public IActionResult Create()
        {
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "AttachmentPath");
            ViewData["StudentId"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: AssignmentSubmissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FilePath,SubmissionDate,AssignmentId,StudentId")] AssignmentSubmission assignmentSubmission)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assignmentSubmission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "AttachmentPath", assignmentSubmission.AssignmentId);
            ViewData["StudentId"] = new SelectList(_context.Users, "Id", "Name", assignmentSubmission.StudentId);
            return View(assignmentSubmission);
        }

        // GET: AssignmentSubmissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentSubmission = await _context.AssignmentSubmissions.FindAsync(id);
            if (assignmentSubmission == null)
            {
                return NotFound();
            }
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "AttachmentPath", assignmentSubmission.AssignmentId);
            ViewData["StudentId"] = new SelectList(_context.Users, "Id", "Name", assignmentSubmission.StudentId);
            return View(assignmentSubmission);
        }

        // POST: AssignmentSubmissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FilePath,SubmissionDate,AssignmentId,StudentId")] AssignmentSubmission assignmentSubmission)
        {
            if (id != assignmentSubmission.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignmentSubmission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentSubmissionExists(assignmentSubmission.Id))
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
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "AttachmentPath", assignmentSubmission.AssignmentId);
            ViewData["StudentId"] = new SelectList(_context.Users, "Id", "Name", assignmentSubmission.StudentId);
            return View(assignmentSubmission);
        }

        // GET: AssignmentSubmissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentSubmission = await _context.AssignmentSubmissions
                .Include(a => a.Assignment)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignmentSubmission == null)
            {
                return NotFound();
            }

            return View(assignmentSubmission);
        }

        // POST: AssignmentSubmissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignmentSubmission = await _context.AssignmentSubmissions.FindAsync(id);
            if (assignmentSubmission != null)
            {
                _context.AssignmentSubmissions.Remove(assignmentSubmission);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentSubmissionExists(int id)
        {
            return _context.AssignmentSubmissions.Any(e => e.Id == id);
        }
    }
}
