using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication_Domain.Entities;
using WebApplication_Infrastructure.Data;

namespace WebApplication_Deneme.Controllers
{
    public class CourseMaterialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseMaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CourseMaterials
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CourseMaterials.Include(c => c.Course).Include(c => c.UploadedBy);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CourseMaterials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseMaterial = await _context.CourseMaterials
                .Include(c => c.Course)
                .Include(c => c.UploadedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseMaterial == null)
            {
                return NotFound();
            }

            return View(courseMaterial);
        }

        // GET: CourseMaterials/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description");
            ViewData["UploadedById"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: CourseMaterials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,Type,FilePath,UploadDate,UploadedById")] CourseMaterial courseMaterial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseMaterial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", courseMaterial.CourseId);
            ViewData["UploadedById"] = new SelectList(_context.Users, "Id", "Name", courseMaterial.UploadedById);
            return View(courseMaterial);
        }

        // GET: CourseMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseMaterial = await _context.CourseMaterials.FindAsync(id);
            if (courseMaterial == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", courseMaterial.CourseId);
            ViewData["UploadedById"] = new SelectList(_context.Users, "Id", "Name", courseMaterial.UploadedById);
            return View(courseMaterial);
        }

        // POST: CourseMaterials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,Type,FilePath,UploadDate,UploadedById")] CourseMaterial courseMaterial)
        {
            if (id != courseMaterial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseMaterialExists(courseMaterial.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", courseMaterial.CourseId);
            ViewData["UploadedById"] = new SelectList(_context.Users, "Id", "Name", courseMaterial.UploadedById);
            return View(courseMaterial);
        }

        // GET: CourseMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseMaterial = await _context.CourseMaterials
                .Include(c => c.Course)
                .Include(c => c.UploadedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseMaterial == null)
            {
                return NotFound();
            }

            return View(courseMaterial);
        }

        // POST: CourseMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseMaterial = await _context.CourseMaterials.FindAsync(id);
            if (courseMaterial != null)
            {
                _context.CourseMaterials.Remove(courseMaterial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseMaterialExists(int id)
        {
            return _context.CourseMaterials.Any(e => e.Id == id);
        }
    }
}
