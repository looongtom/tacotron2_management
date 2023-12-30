using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tacotron2_management.Models;

namespace tacotron2_management.Controllers
{
    public class ExpertListenersController : Controller
    {
        private readonly TacotronTtsContext _context;

        public ExpertListenersController(TacotronTtsContext context)
        {
            _context = context;
        }

        // GET: ExpertListeners
        public async Task<IActionResult> Index()
        {
              return _context.ExpertListeners != null ? 
                          View(await _context.ExpertListeners.ToListAsync()) :
                          Problem("Entity set 'TacotronTtsContext.ExpertListeners'  is null.");
        }

        // GET: ExpertListeners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ExpertListeners == null)
            {
                return NotFound();
            }

            var expertListener = await _context.ExpertListeners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expertListener == null)
            {
                return NotFound();
            }

            return View(expertListener);
        }

        // GET: ExpertListeners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExpertListeners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ExpertListener expertListener)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expertListener);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expertListener);
        }

        // GET: ExpertListeners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ExpertListeners == null)
            {
                return NotFound();
            }

            var expertListener = await _context.ExpertListeners.FindAsync(id);
            if (expertListener == null)
            {
                return NotFound();
            }
            return View(expertListener);
        }

        // POST: ExpertListeners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ExpertListener expertListener)
        {
            if (id != expertListener.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expertListener);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpertListenerExists(expertListener.Id))
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
            return View(expertListener);
        }

        // GET: ExpertListeners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ExpertListeners == null)
            {
                return NotFound();
            }

            var expertListener = await _context.ExpertListeners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expertListener == null)
            {
                return NotFound();
            }

            return View(expertListener);
        }

        // POST: ExpertListeners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ExpertListeners == null)
            {
                return Problem("Entity set 'TacotronTtsContext.ExpertListeners'  is null.");
            }
            var expertListener = await _context.ExpertListeners.FindAsync(id);
            if (expertListener != null)
            {
                _context.ExpertListeners.Remove(expertListener);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpertListenerExists(int id)
        {
          return (_context.ExpertListeners?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
