using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.WebPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using tacotron2_management.Models;

namespace tacotron2_management.Controllers
{
    public class TblModelsController : Controller
    {
        private readonly TacotronTtsContext _context;
        private string connString = @"Server =.\CSDLPTTEST; Database = tacotron_tts;Persist Security Info=True;User ID=sa;Password=88888888;TrustServerCertificate=True; Trusted_Connection = True;";

        public TblModelsController(TacotronTtsContext context)
        {
            _context = context;
        }

        // GET: TblModels
        public async Task<IActionResult> Index(DateTime startDate, DateTime endDate, string? messageType)
        {
            var listModels = from s in _context.TblModels select s;
            listModels = listModels.OrderByDescending(a => a.TrainDate);

            DateTime currentDateTime = DateTime.Now;

            if (startDate > currentDateTime)
            {
                return NotFound();
            }

            if (startDate < endDate)
            {
                HttpContext.Session.SetString("startDate", startDate.ToString());
                HttpContext.Session.SetString("endDate", endDate.ToString());

                listModels = listModels.Where(s => s.TrainDate >= startDate && s.TrainDate <= endDate);
            }
            if (messageType == null)
            {
                messageType = "None";
            }
            HttpContext.Session.SetString("messageType", messageType);

            return View(await listModels.ToListAsync());

        }

        // GET: TblModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TblModels == null)
            {
                return NotFound();
            }

            var tblModel = await _context.TblModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblModel == null)
            {
                return NotFound();
            }

            return View(tblModel);
        }

        // GET: TblModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TblModel tblModel)
        {

            DateTime currentTime = DateTime.Now;
            tblModel.TrainDate = currentTime;
            tblModel.Status = "Is training";
            _context.Add(tblModel);
            await _context.SaveChangesAsync();

            int modelId = tblModel.Id;
            HttpContext.Session.SetInt32("modelId", modelId);

            return RedirectToAction("ChooseTrainFileForModel", "tblTrains");
        }

        public async Task<IActionResult> ChooseTrainFileVer2(int modelId)
        {
            HttpContext.Session.SetInt32("modelId", modelId);

            return RedirectToAction("ChooseTrainFileForModel", "tblTrains");
        }
        public async Task<IActionResult> ChooseTrainFileForModel(int id)
        {
            int modelId = (int)(HttpContext.Session.GetInt32("modelId") ?? -1);
            if (modelId != -1)
            {
                var tblModel = await _context.TblModels.FindAsync(modelId);
                if (tblModel == null)
                {
                    return NotFound();
                }
                _context.Update(tblModel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "tblModels");
        }

        // GET: TblModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TblModels == null)
            {
                return NotFound();
            }

            var tblModel = await _context.TblModels.FindAsync(id);
            if (tblModel == null)
            {
                return NotFound();
            }
            return View(tblModel);
        }

        // POST: TblModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Url,TrainDate,MosAverage,Status,TblTrainId")] TblModel tblModel)
        {
            if (id != tblModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblModelExists(tblModel.Id))
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
            return View(tblModel);
        }

        public async Task<IActionResult> IndexRetrain(String messageType)
        {
            var listModels = from s in _context.TblModels select s;
            listModels = listModels.OrderByDescending(a => a.TrainDate);

            DateTime currentDateTime = DateTime.Now;

            HttpContext.Session.SetString("messageType", messageType);
            HttpContext.Session.SetString("messageType", messageType);


            return View(await listModels.ToListAsync());

        }
        [HttpPost]
        public async Task<IActionResult> RefreshMessage()
        {
            GlobalMessage.SetGlobalStr("None");
            return RedirectToAction("Index", "tblModels");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateURL(int id, string url, double lossAvg)
        {
            var findModel = await _context.TblModels.FindAsync(id);
            GlobalModelName.SetGlobalModel(findModel.Name);
            double previousLossAvg = (double)findModel.MosAverage;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.Now;
            string messageType;

            if (lossAvg > previousLossAvg)
            {
                if (findModel != null)
                {
                    findModel.Status = "Completely trained";
                }
                _context.Update(findModel);
                await _context.SaveChangesAsync();

                messageType = "Warning";
                GlobalMessage.SetGlobalStr("Warning");

                return RedirectToAction("Index", "tblModels");
            }

            if (findModel != null)
            {
                findModel.Url = url;
                findModel.Status = "Completely trained";
                findModel.TrainDate = DateTime.Now;
                findModel.MosAverage = lossAvg;
            }
            _context.Update(findModel);
            await _context.SaveChangesAsync();

            messageType = "Success";
            GlobalMessage.SetGlobalStr("Success");
            return RedirectToAction(nameof(Index));

        }

        //ChooseAsDefault
        [HttpPost]
        public async Task<IActionResult> ChooseAsDefault(int id)
        {
            var chooseModel = await _context.TblModels.FindAsync(id);
            if (chooseModel == null)
            {
                return NotFound();
            }
            var listModels = from s in _context.TblModels select s;
            var previousDefaultModel = listModels.Where(s => s.Status == "Default").FirstOrDefault();

            if (previousDefaultModel != null)
            {
                previousDefaultModel.Status = "Completely trained";
                _context.Update(previousDefaultModel);
            }
            chooseModel.Status = "Default";
            _context.Update(chooseModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "tblModels");
        }

        // GET: TblModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TblModels == null)
            {
                return NotFound();
            }

            var tblModel = await _context.TblModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblModel == null)
            {
                return NotFound();
            }

            return View(tblModel);
        }

        // POST: TblModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TblModels == null)
            {
                return Problem("Entity set 'TacotronTtsContext.TblModels'  is null.");
            }
            //find tblTrain with modelId = id
            var tblTrain = await _context.TblTrains.Where(s => s.TblModelId == id).FirstOrDefaultAsync();
            //update tblTrain with modelId = null
            if (tblTrain != null)
            {
                tblTrain.TblModelId = null;
                _context.Update(tblTrain);
            }
            await _context.SaveChangesAsync();

            var tblModel = await _context.TblModels.FindAsync(id);
            if (tblModel != null)
            {
                _context.TblModels.Remove(tblModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Retrain(int? id)
        {
            if (id == null || _context.TblModels == null)
            {
                return NotFound();
            }

            var tblModel = await _context.TblModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblModel == null)
            {
                return NotFound();
            }

            // get tblTrain with modelId = id
            var tblTrain = await _context.TblTrains.Where(s => s.TblModelId == id).FirstOrDefaultAsync();

            if (tblTrain == null)
            {
                //return NotFound("Model does not contain any train file");
                ModelDetails modelDetailsWihtoutTrain = new ModelDetails()
                {
                    ListAudio = null,
                    ListCheckUntrained = null,
                    Model = tblModel,
                    Train = null
                };
                return View(modelDetailsWihtoutTrain);
            }

            string query = "SELECT tblAudioId FROM [tacotron_tts].[dbo].[tblAudio_tblTrain] WHERE [tblTrainId] = " + tblTrain.Id;
            List<int> listTrainedAudioId = new List<int>();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                // Open the connection
                connection.Open();

                // Create a command with the SQL query and connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Execute the SQL query and get the result
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if there are rows returned
                        if (reader.HasRows)
                        {
                            // Iterate through the rows and print the tblAudioId values
                            while (reader.Read())
                            {
                                int tblAudioId = reader.GetInt32(0); // Assuming tblAudioId is of type int
                                listTrainedAudioId.Add(tblAudioId);
                                System.Diagnostics.Debug.WriteLine($"tblAudioId: {tblAudioId}");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("No rows found.");
                        }
                    }
                }
            }

            List<TblAudio> listTrainedAudio = new List<TblAudio>();
            foreach (int audioId in listTrainedAudioId)
            {
                var audio = await _context.TblAudios.FindAsync(audioId);
                if (audio != null)
                {
                    listTrainedAudio.Add(audio);
                }
            }

            foreach (var item in listTrainedAudio.ToList())
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
                    }
                }
                Debug.WriteLine(item.ToString);
            }

            List<TblAudio> listUnTrainedAudio = new List<TblAudio>();
            foreach (TblAudio audio in _context.TblAudios)
            {
                if (!listTrainedAudioId.Contains(audio.Id))
                {
                    listUnTrainedAudio.Add(audio);
                }
            }
            foreach (var item in listUnTrainedAudio.ToList())
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
                    }
                }
                Debug.WriteLine(item.ToString);
            }

            List<int> listCheckUntrained = new List<int>();
            foreach (TblAudio audio in listUnTrainedAudio)
            {
                listCheckUntrained.Add(audio.Id);
            }

            List<TblAudio> listAllAudio = new List<TblAudio>(listUnTrainedAudio);
            listAllAudio.AddRange(listTrainedAudio);

            var modelDetails = new ModelDetails
            {
                Train = tblTrain,
                ListAudio = listAllAudio,
                ListCheckUntrained = listCheckUntrained,
                Model = tblModel
            };

            //set listTrainedAudioId into session
            //HttpContext.Current.Session.Add("currentUser", listTrainedAudioId);
            HttpContext.Session.SetString("previous_fileTrain", tblTrain.Folder);
            HttpContext.Session.SetString("previous_modelURL", tblModel.Url);
            HttpContext.Session.SetString("previous_lossAverage", tblModel.MosAverage.ToString());



            return View(modelDetails);
        }

        [HttpPost]
        public async Task<IActionResult> RetrainModel(int idTrain, int idModel)
        {
            var tblTrain = await _context.TblTrains.FindAsync(idTrain);
            var tblModel = await _context.TblModels.FindAsync(idModel);
            var lossAvg = tblModel.MosAverage;

            tblTrain.TblModelId = idModel;
            _context.Update(tblTrain);

            tblModel.Status = "Is training";
            _context.Update(tblModel);

            await _context.SaveChangesAsync();

            String apiUrl = "http://" + getCurrentIp() + ":5000/run_tacotron2";
            string folder = tblTrain.Folder;
            string folderName = getFolderName(folder);

            HttpClient client = new HttpClient();
            string requestUrl = $"{apiUrl}?meta_file_train={folderName} & id_model={idModel} & previous_loss={lossAvg}";

            client.GetAsync(requestUrl);
            return RedirectToAction("Index", "TblModels");

        }
        public static string getCurrentIp()
        {
            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && networkInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        UnicastIPAddressInformationCollection ipAddresses = networkInterface.GetIPProperties().UnicastAddresses;

                        foreach (UnicastIPAddressInformation ipAddress in ipAddresses)
                        {
                            if (ipAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                return ipAddress.Address.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return null;
        }

        public static string getFolderName(string input)
        {
            string pattern = @"/([^/]+)$";

            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                string lastSubstring = match.Groups[1].Value;
                return lastSubstring;
            }
            else
            {
                return "";
            }
        }


        private bool TblModelExists(int id)
        {
            return (_context.TblModels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
