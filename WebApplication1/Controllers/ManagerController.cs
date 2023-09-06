using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ManagerController : Controller
    {
        private readonly WebApplication1Context _context;

        public ManagerController(WebApplication1Context context)
        {
            _context = context;
        }

        // GET: Flavours1
        public async Task<IActionResult> Index()
        {
              return _context.Flavour != null ? 
                          View(await _context.Flavour.ToListAsync()) :
                          Problem("Entity set 'WebApplication1Context.Flavour'  is null.");
        }

        // GET: Flavours1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Flavour == null)
            {
                return NotFound();
            }

            var flavour = await _context.Flavour
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flavour == null)
            {
                return NotFound();
            }

            return View(flavour);
        }

        // GET: Flavours1/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Flavours1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,ImageURL")] Flavour flavour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flavour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flavour);
        }

        // GET: Flavours1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Flavour == null)
            {
                return NotFound();
            }

            var flavour = await _context.Flavour.FindAsync(id);
            if (flavour == null)
            {
                return NotFound();
            }
            return View(flavour);
        }

        // POST: Flavours1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageURL")] Flavour flavour)
        {
            if (id != flavour.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flavour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlavourExists(flavour.Id))
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
            return View(flavour);
        }

        // GET: Flavours1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Flavour == null)
            {
                return NotFound();
            }

            var flavour = await _context.Flavour
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flavour == null)
            {
                return NotFound();
            }

            return View(flavour);
        }

        // POST: Flavours1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Flavour == null)
            {
                return Problem("Entity set 'WebApplication1Context.Flavour'  is null.");
            }
            var flavour = await _context.Flavour.FindAsync(id);
            if (flavour != null)
            {
                _context.Flavour.Remove(flavour);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlavourExists(int id)
        {
          return (_context.Flavour?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
