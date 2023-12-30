using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NAudio.Wave;
using tacotron2_management.Models;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace tacotron2_management.Controllers
{
    public class TblAudiosController : Controller
    {
        private readonly TacotronTtsContext _context;

        public TblAudiosController(TacotronTtsContext context)
        {
            _context = context;
        }

        // GET: TblAudios
        public async Task<IActionResult> Index( string searchString, string currentFilter, int? page)
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

            var listAudios = from s in _context.TblAudios select s;
            listAudios=listAudios.OrderByDescending(a => a.Id);
            if (!String.IsNullOrEmpty(searchString))
            {
                listAudios=listAudios.Where(s=>s.AudioName.Contains(searchString));
            }
            List<TblAudio> listUpdateAudio= new List<TblAudio>();
            foreach (var item in listAudios.ToList())
            {
                // Assuming 'ColumnName' is the name of the column you want to set to null

                //if (item.MosScore == null) item.MosScore = -1;
                if (item.TblTranscriptId == null) item.TblTranscriptId = -1;
                else
                {
                    TblTranscript transcript = _context.Find<TblTranscript>(item.TblTranscriptId);
                    if (transcript != null)
                    {
                        item.TblTranscript = transcript;
                        listUpdateAudio.Add(item);
                    }
                }
                Debug.WriteLine(item.ToString);
            }
            IQueryable<TblAudio> updatedQuery = listAudios.Select(item => new TblAudio
            {
                Id = item.Id,  // Copy the ID as is
                               // Update the desired property conditionally
                TblTranscript = item.TblTranscript,
                // Copy other properties as needed
                AudioName = item.AudioName,
                Duration = item.Duration,
                Size = item.Size,
                IsTrained = item.IsTrained,
                URL = item.URL,
                TblTranscriptId = item.TblTranscriptId
            });

                int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(updatedQuery.ToPagedList(pageNumber, pageSize));
        }

       
        public async Task<IActionResult> ChooseAudioForTrain(string searchString, string currentFilter, int? page)
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

            var listAudios = from s in _context.TblAudios select s;

            var countUntrainedAudios = _context.TblAudios.Where(audio => audio.IsTrained == false).Count();
            ViewBag.TotalUnTrainedAudio = countUntrainedAudios;

            //listAudios= listAudios.Where(a => a.IsTrained == false); // Đếm số audio chưa train
            listAudios = listAudios.OrderByDescending(a => a.Id);
            if (!String.IsNullOrEmpty(searchString))
            {
                listAudios = listAudios.Where(s => s.AudioName.Contains(searchString));
            }
            List<TblAudio> listUpdateAudio = new List<TblAudio>();
            foreach (var item in listAudios.ToList())
            {
                // Assuming 'ColumnName' is the name of the column you want to set to null

                //if (item.MosScore == null) item.MosScore = -1;
                if (item.TblTranscriptId == null) item.TblTranscriptId = -1;
                else
                {
                    TblTranscript transcript = _context.Find<TblTranscript>(item.TblTranscriptId);
                    if (transcript != null)
                    {
                        item.TblTranscript = transcript;
                        listUpdateAudio.Add(item);
                    }
                }
                Debug.WriteLine(item.ToString);
            }
            IQueryable<TblAudio> updatedQuery = listAudios.Select(item => new TblAudio
            {
                Id = item.Id,  // Copy the ID as is
                               // Update the desired property conditionally
                TblTranscript = item.TblTranscript,
                // Copy other properties as needed
                AudioName = item.AudioName,
                Duration = item.Duration,
                Size = item.Size,
                IsTrained = item.IsTrained,
                URL = item.URL,
                TblTranscriptId = item.TblTranscriptId
            });

            int pageSize = 13100;
            int pageNumber = (page ?? 1);
            return View(updatedQuery.ToPagedList(pageNumber, pageSize));
        }

        // GET: TblAudios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TblAudios == null)
            {
                return NotFound();
            }

            var tblAudio = await _context.TblAudios
                .Include(t => t.TblTranscript)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblAudio == null)
            {
                return NotFound();
            }

            return View(tblAudio);
        }

        // GET: TblAudios/Create
        public IActionResult Create()
        {
            ViewData["TblTranscriptId"] = new SelectList(_context.TblTranscripts, "Id", "Id");
            return View();
        }

        // POST: TblAudios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TblTranscriptId,AudioName,Duration,Size,IsTrained")] TblAudio tblAudio)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add(tblAudio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            ViewData["TblTranscriptId"] = new SelectList(_context.TblTranscripts, "Id", "Id", tblAudio.TblTranscriptId);
            return View(tblAudio);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(SingleFileModel model)
        {
            model.IsResponse = true;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/wavs");
            //create folder if not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //get file extension
            FileInfo fileInfo = new FileInfo(model.File.FileName);
            string fileName = fileInfo.Name;
            
            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                model.File.CopyTo(stream);
            }
            double duration = getDuration(fileNameWithPath);

            model.IsSuccess = true;
            model.Message = "File upload successfully";

            TblAudio tblAudio = null;
            tblAudio = new TblAudio();
            if (fileName.Contains(".wav"))
            {
                fileName = fileName.Replace(".wav", "");
            }
            tblAudio.AudioName = fileName;
            tblAudio.Duration = (float)duration;
            tblAudio.Size = (float)new FileInfo(fileNameWithPath).Length;
            tblAudio.IsTrained = false;

            string searchString = "wwwroot";

            int index = fileNameWithPath.IndexOf(searchString);

            if (index != -1)
            {
                fileNameWithPath = fileNameWithPath.Substring(index + searchString.Length);
            }
            tblAudio.URL =fileNameWithPath;
            tblAudio.TblTranscriptId = model.TranscriptID;

            _context.Add(tblAudio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Choose(int audioID,int transcriptID)
        {
            HttpContext.Session.SetInt32("SelectedAudioId", audioID);
            HttpContext.Session.SetInt32("SelectedTranscriptId", transcriptID);
            TblTranscript transcript = _context.Find<TblTranscript>(transcriptID);
            HttpContext.Session.SetString("ContentTranscript", transcript.Content);

            return RedirectToAction("UpdateAudio", "tblAudios");
        }

        public IActionResult UpdateAudio()
        {
            ViewData["TblTranscriptId"] = new SelectList(_context.TblTranscripts, "Id", "Id");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAudio(int id,int transcriptID, SingleFileModel model)
        {
            if (model == null ||model.File==null)
            {
                var oldAudio = _context.Find<TblAudio>(id);
                TblAudio tblNewAudio = null;
                tblNewAudio = new TblAudio();
                tblNewAudio.Id = id;
                tblNewAudio.AudioName = oldAudio.AudioName;
                tblNewAudio.Duration = oldAudio.Duration;
                tblNewAudio.Size = oldAudio.Size;
                tblNewAudio.URL= oldAudio.URL;
                tblNewAudio.TblTranscriptId = transcriptID;
                try
                {
                    _context.Update(tblNewAudio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblAudioExists(tblNewAudio.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/wavs");
                //create folder if not exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //get file extension
                FileInfo fileInfo = new FileInfo(model.File.FileName);
                string fileName = fileInfo.Name;
                string fileNameWithPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    model.File.CopyTo(stream);
                }
                double duration = getDuration(fileNameWithPath);

                var previousAudio = _context.Find<TblAudio>(id);

                TblAudio tblAudio = null;
                tblAudio = new TblAudio();
                tblAudio.Id = id;
                if (fileName.Contains(".wav"))
                {
                    fileName = fileName.Replace(".wav", "");
                }
                tblAudio.AudioName = fileName;
                tblAudio.Duration = (float)duration;
                tblAudio.Size = (float)new FileInfo(fileNameWithPath).Length;
                tblAudio.IsTrained = false;
                string searchString = "wwwroot";

                int index = fileNameWithPath.IndexOf(searchString);

                if (index != -1)
                {
                    fileNameWithPath = fileNameWithPath.Substring(index + searchString.Length);
                }
                tblAudio.URL = fileNameWithPath;
                tblAudio.TblTranscriptId = transcriptID;

                Debug.WriteLine(tblAudio.ToString);

                try
                {
                    _context.Update(tblAudio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblAudioExists(tblAudio.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Index));

        }

        public static double getDuration(string fileName)
        {
            WaveFileReader wf = new WaveFileReader(fileName);
            return wf.TotalTime.TotalSeconds;
        }

        public IActionResult ChooseTranscript()
        {
            return RedirectToAction("ViewToChoose", "TblTranscripts");
        }
        public async Task<IActionResult> ChooseTranscriptUpdate(int audioID,int transcriptID)
        {
            HttpContext.Session.SetInt32("SelectedAudioId", audioID);
            HttpContext.Session.SetInt32("SelectedTranscriptId", transcriptID);
            TblTranscript transcript = _context.Find<TblTranscript>(transcriptID);
            HttpContext.Session.SetString("ContentTranscript", transcript.Content);

            return RedirectToAction("ViewToChooseUpdate", "TblTranscripts");
        }

        // GET: TblAudios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TblAudios == null)
            {
                return NotFound();
            }

            var tblAudio = await _context.TblAudios.FindAsync(id);
            if (tblAudio == null)
            {
                return NotFound();
            }
            ViewData["TblTranscriptId"] = new SelectList(_context.TblTranscripts, "Id", "Id", tblAudio.TblTranscriptId);
            return View(tblAudio);
        }

        // POST: TblAudios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TblTranscriptId,AudioName,Duration,Size,IsTrained")] TblAudio tblAudio)
        {
            if (id != tblAudio.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(tblAudio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblAudioExists(tblAudio.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            
        }

        // GET: TblAudios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TblAudios == null)
            {
                return NotFound();
            }

            var tblAudio = await _context.TblAudios
                .Include(t => t.TblTranscript)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblAudio == null)
            {
                return NotFound();
            }

            return View(tblAudio);
        }

        // POST: TblAudios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TblAudios == null)
            {
                return Problem("Entity set 'TacotronTtsContext.TblAudios'  is null.");
            }
            var tblAudio = await _context.TblAudios.FindAsync(id);
            if (tblAudio != null)
            {
                _context.TblAudios.Remove(tblAudio);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblAudioExists(int id)
        {
          return (_context.TblAudios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
