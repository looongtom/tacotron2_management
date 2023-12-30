using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tacotron2_management.Models;
using X.PagedList;

namespace tacotron2_management.Controllers
{
    public class TblTranscriptsController : Controller
    {
        private readonly TacotronTtsContext _context;

        public TblTranscriptsController(TacotronTtsContext context)
        {
            _context = context;
        }

        // GET: TblTranscripts
        public async Task<IActionResult> Index(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            ViewBag.CurrentFilter = searchString;

            var transcripts = from s in _context.TblTranscripts select s;
            transcripts=transcripts.OrderByDescending(a => a.Id);
            if (!String.IsNullOrEmpty(searchString))
            {
                transcripts = transcripts.Where(s => s.Content.Contains(searchString));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(transcripts.ToPagedList(pageNumber,pageSize));
              //return _context.TblTranscripts != null ? 
              //            View(await _context.TblTranscripts.ToListAsync()) :
              //            Problem("Entity set 'TacotronTtsContext.TblTranscripts'  is null.");
        }

        public async Task<IActionResult> ViewToChoose(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            ViewBag.CurrentFilter = searchString;

            var transcripts = from s in _context.TblTranscripts select s;
            transcripts = transcripts.OrderByDescending(a => a.Id);
            if (!String.IsNullOrEmpty(searchString))
            {
                transcripts = transcripts.Where(s => s.Content.Contains(searchString));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(transcripts.ToPagedList(pageNumber, pageSize));
            //return _context.TblTranscripts != null ? 
            //            View(await _context.TblTranscripts.ToListAsync()) :
            //            Problem("Entity set 'TacotronTtsContext.TblTranscripts'  is null.");
        }

        public async Task<IActionResult> ViewToChooseUpdate(string searchString, string currentFilter, int? page)
        {


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            ViewBag.CurrentFilter = searchString;

            var transcripts = from s in _context.TblTranscripts select s;
            transcripts = transcripts.OrderByDescending(a => a.Id);
            if (!String.IsNullOrEmpty(searchString))
            {
                transcripts = transcripts.Where(s => s.Content.Contains(searchString));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(transcripts.ToPagedList(pageNumber, pageSize));
            //return _context.TblTranscripts != null ? 
            //            View(await _context.TblTranscripts.ToListAsync()) :
            //            Problem("Entity set 'TacotronTtsContext.TblTranscripts'  is null.");
        }

        public async Task<IActionResult> Choose(int transcriptID)
        {
            HttpContext.Session.SetInt32("SelectedTranscriptId", transcriptID);

            return RedirectToAction("Create", "tblAudios");
        }

        public async Task<IActionResult> ChooseUpdate(int transcriptID,int audioID)
        {
            HttpContext.Session.SetInt32("SelectedTranscriptId", transcriptID);
            HttpContext.Session.SetInt32("SelectedAudioId", audioID);
            TblTranscript transcript = _context.Find<TblTranscript>(transcriptID);
            HttpContext.Session.SetString("ContentTranscript", transcript.Content);
            //return RedirectToAction("Update", "tblAudios");
            return RedirectToAction("Choose", "tblAudios", new { audioID = audioID, transcriptID = transcriptID });

        }

        // GET: TblTranscripts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TblTranscripts == null)
            {
                return NotFound();
            }

            var tblTranscript = await _context.TblTranscripts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblTranscript == null)
            {
                return NotFound();
            }

            return View(tblTranscript);
        }

        // GET: TblTranscripts/Create
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult UpdatedTrainedTranscript()
        {
            var listTranscript = from s in _context.TblTranscripts select s;
            foreach (TblTranscript transcript in listTranscript)
            {
                transcript.IsTrained = true;
                _context.Update(transcript);
            }
            _context.SaveChangesAsync();

            return RedirectToAction("ChooseAudioForTrain", "tblAudios");

        }

        // POST: TblTranscripts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,IsTrained")] TblTranscript tblTranscript)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblTranscript);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblTranscript);
        }

        // GET: TblTranscripts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TblTranscripts == null)
            {
                return NotFound();
            }

            var tblTranscript = await _context.TblTranscripts.FindAsync(id);
            if (tblTranscript == null)
            {
                return NotFound();
            }
            return View(tblTranscript);
        }

        // POST: TblTranscripts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,IsTrained")] TblTranscript tblTranscript)
        {
            if (id != tblTranscript.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblTranscript);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblTranscriptExists(tblTranscript.Id))
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
            return View(tblTranscript);
        }

        // GET: TblTranscripts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TblTranscripts == null)
            {
                return NotFound();
            }

            var tblTranscript = await _context.TblTranscripts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblTranscript == null)
            {
                return NotFound();
            }

            return View(tblTranscript);
        }

        // POST: TblTranscripts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TblTranscripts == null)
            {
                return Problem("Entity set 'TacotronTtsContext.TblTranscripts'  is null.");
            }
            var tblTranscript = await _context.TblTranscripts.FindAsync(id);
            if (tblTranscript != null)
            {
                _context.TblTranscripts.Remove(tblTranscript);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblTranscriptExists(int id)
        {
          return (_context.TblTranscripts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
