using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_Infrastructure.Data;
using WebApplication_Domain.Entities;

namespace WebApplication_Deneme.Controllers
{
    public class TeacherBranchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherBranchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TeacherBranches
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TeacherBranches.Include(t => t.Branch).Include(t => t.Teacher);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TeacherBranches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherBranch = await _context.TeacherBranches
                .Include(t => t.Branch)
                .Include(t => t.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacherBranch == null)
            {
                return NotFound();
            }

            return View(teacherBranch);
        }

        // GET: TeacherBranches/Create
        public IActionResult Create()
        {
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Id");
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: TeacherBranches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TeacherId,BranchId")] TeacherBranch teacherBranch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacherBranch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Id", teacherBranch.BranchId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", teacherBranch.TeacherId);
            return View(teacherBranch);
        }

        // GET: TeacherBranches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherBranch = await _context.TeacherBranches.FindAsync(id);
            if (teacherBranch == null)
            {
                return NotFound();
            }
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Id", teacherBranch.BranchId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", teacherBranch.TeacherId);
            return View(teacherBranch);
        }

        // POST: TeacherBranches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeacherId,BranchId")] TeacherBranch teacherBranch)
        {
            if (id != teacherBranch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacherBranch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherBranchExists(teacherBranch.Id))
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
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Id", teacherBranch.BranchId);
            ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", teacherBranch.TeacherId);
            return View(teacherBranch);
        }

        // GET: TeacherBranches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacherBranch = await _context.TeacherBranches
                .Include(t => t.Branch)
                .Include(t => t.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacherBranch == null)
            {
                return NotFound();
            }

            return View(teacherBranch);
        }

        // POST: TeacherBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacherBranch = await _context.TeacherBranches.FindAsync(id);
            if (teacherBranch != null)
            {
                _context.TeacherBranches.Remove(teacherBranch);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherBranchExists(int id)
        {
            return _context.TeacherBranches.Any(e => e.Id == id);
        }
    }
}
