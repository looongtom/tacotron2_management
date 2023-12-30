using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using tacotron2_management.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace tacotron2_management.Controllers
{
    public class TblTrainsController : Controller
    {
        private readonly TacotronTtsContext _context;
        private string connString = @"Server =.\CSDLPTTEST; Database = tacotron_tts;Persist Security Info=True;User ID=sa;Password=88888888;TrustServerCertificate=True; Trusted_Connection = True;";

        public TblTrainsController(TacotronTtsContext context)
        {
            _context = context;
        }

        // GET: TblTrains
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblTrains.OrderByDescending(t => t.Id).ToListAsync());
        }

        // GET: TblTrains/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TblTrains == null)
            {
                return NotFound();
            }

            TblTrain tblTrain = await _context.TblTrains
                .FirstOrDefaultAsync(m => m.Id == id);

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


            List<TblTranscript> listTranscript = new List<TblTranscript>();
            foreach (TblAudio audio in listTrainedAudio)
            {
                var transcript = await _context.TblTranscripts.FindAsync(audio.TblTranscriptId);
                if (transcript != null)
                {
                    listTranscript.Add(transcript);
                }
            }

            var viewModel = new TrainDetailsViewModel
            {
                Train = tblTrain,
                ListAudio = listTrainedAudio,
                ListTranscript = listTranscript,
            };


            if (tblTrain == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        public async Task<IActionResult> TrainDetailsUpdate(int id)
        {
            if (id == null || _context.TblTrains == null)
            {
                return NotFound();
            }

            TblTrain tblTrain = await _context.TblTrains
                .FirstOrDefaultAsync(m => m.Id == id);

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


            List<TblTranscript> listTranscript = new List<TblTranscript>();
            foreach (TblAudio audio in listTrainedAudio)
            {
                var transcript = await _context.TblTranscripts.FindAsync(audio.TblTranscriptId);
                if (transcript != null)
                {
                    listTranscript.Add(transcript);
                }
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

            var viewModel = new TrainDetailsViewModel
            {
                Train = tblTrain,
                ListAudio = listAllAudio,
                ListTranscript = listTranscript,
                ListCheckUntrained = listCheckUntrained,
            };


            if (tblTrain == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }


        // GET: TblTrains/Create
        //public IActionResult Create()
        //{
        //    ViewData["TblModelId"] = new SelectList(_context.TblModels, "Id", "Id");
        //    return View();
        //}

        public IActionResult ChooseAudioForTrain()
        {
            return RedirectToAction("ChooseAudioForTrain", "TblAudios");
        }

        // POST: TblTrains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public async Task<IActionResult> Create()
        {
            DateTime currentTime = DateTime.Now;
            long unixTimestamp = (long)(currentTime - new DateTime(1970, 1, 1)).TotalMilliseconds;

            var listAudios = from s in _context.TblAudios select s;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/" + unixTimestamp.ToString() + "list.txt");
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
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (TblAudio audio in updatedQuery)
                {
                    string audioName = audio.AudioName;
                    string transcript = audio.TblTranscript.Content;
                    writer.WriteLine(audioName + "|" + transcript + "|" + transcript);
                }
            }

            foreach (TblAudio audio in updatedQuery)
            {
                audio.IsTrained = true;
                _context.Update(audio);
            }
            //await _context.SaveChangesAsync();

            TblTrain newTrain = new TblTrain();
            newTrain.Folder = filePath;
            _context.Add(newTrain);

            await _context.SaveChangesAsync();


            return RedirectToAction("UpdatedTrainedTranscript", "tblTranscripts");
        }

        public async Task<IActionResult> CustomUpdate(int idTrain, List<int> selectedTrainedItems)
        {
            Debug.WriteLine("idTrain: " + idTrain);
            Debug.WriteLine("idTrain: " + selectedTrainedItems.Count);
            TblTrain tblTrain = await _context.TblTrains
                .FirstOrDefaultAsync(m => m.Id == idTrain);
            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    String queryDelete = "DELETE FROM [tacotron_tts].[dbo].[tblAudio_tblTrain] WHERE [tblTrainId] = " + tblTrain.Id;
                    SqlCommand sqlCommand = new SqlCommand(queryDelete, connection);
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    Console.WriteLine("DELETE statement successfully executed.");

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            foreach (int id in selectedTrainedItems)
            {
                var audio = await _context.TblAudios.FindAsync(id);
                if (audio == null)
                {
                    continue;
                }
                audio.IsTrained = true;
                _context.Update(audio); // đánh dấu audio đã được train
                try
                {
                    using (SqlConnection connection = new SqlConnection(connString))
                    {
                        String queryInsert = "INSERT INTO [tacotron_tts].[dbo].[tblAudio_tblTrain] ([tblAudioId],[tblTrainId]) VALUES (" + id + "," + idTrain + ")";
                        SqlCommand sqlCommand = new SqlCommand(queryInsert, connection);
                        connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        connection.Close();
                        Console.WriteLine("INSERT statement successfully executed.");

                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            // Tạo list audio đã được train
            IQueryable<TblAudio> listAudios = null;
            foreach (int id in selectedTrainedItems)
            {
                var audio = await _context.TblAudios.FindAsync(id);
                if (audio == null)
                {
                    continue;
                }
                if (listAudios == null)
                {
                    listAudios = from s in _context.TblAudios where s.Id == id select s;
                }
                else
                {
                    listAudios = listAudios.Union(from s in _context.TblAudios where s.Id == id select s);
                }
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
            //Lưu file text
            DateTime currentTime = DateTime.Now;
            long unixTimestamp = (long)(currentTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/" + unixTimestamp.ToString() + "list.txt");
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (TblAudio audio in updatedQuery)
                {
                    string audioName = audio.AudioName;
                    string transcript = audio.TblTranscript.Content;
                    writer.WriteLine(audioName + "|" + transcript + "|" + transcript);
                }
            }
            //Update file text vào tblTrain
            tblTrain.Folder = filePath;
            _context.Update(tblTrain);
            await _context.SaveChangesAsync();


            return RedirectToAction("Details", "TblTrains", new { id = idTrain }); // Replace 123 with the actual ID value
        }

        public async Task<IActionResult> CustomUpdateBeforeRetrain(int idTrain, List<int> selectedTrainedItems)
        {
            Debug.WriteLine("idTrain: " + idTrain);
            Debug.WriteLine("idTrain: " + selectedTrainedItems.Count);
            TblTrain tblTrain = await _context.TblTrains
                .FirstOrDefaultAsync(m => m.Id == idTrain);
            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    String queryDelete = "DELETE FROM [tacotron_tts].[dbo].[tblAudio_tblTrain] WHERE [tblTrainId] = " + tblTrain.Id;
                    SqlCommand sqlCommand = new SqlCommand(queryDelete, connection);
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    Console.WriteLine("DELETE statement successfully executed.");

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            foreach (int id in selectedTrainedItems)
            {
                var audio = await _context.TblAudios.FindAsync(id);
                if (audio == null)
                {
                    continue;
                }
                audio.IsTrained = true;
                _context.Update(audio); // đánh dấu audio đã được train
                try
                {
                    using (SqlConnection connection = new SqlConnection(connString))
                    {
                        String queryInsert = "INSERT INTO [tacotron_tts].[dbo].[tblAudio_tblTrain] ([tblAudioId],[tblTrainId]) VALUES (" + id + "," + idTrain + ")";
                        SqlCommand sqlCommand = new SqlCommand(queryInsert, connection);
                        connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        connection.Close();
                        Console.WriteLine("INSERT statement successfully executed.");

                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            // Tạo list audio đã được train
            IQueryable<TblAudio> listAudios = null;
            foreach (int id in selectedTrainedItems)
            {
                var audio = await _context.TblAudios.FindAsync(id);
                if (audio == null)
                {
                    continue;
                }
                if (listAudios == null)
                {
                    listAudios = from s in _context.TblAudios where s.Id == id select s;
                }
                else
                {
                    listAudios = listAudios.Union(from s in _context.TblAudios where s.Id == id select s);
                }
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
            //Lưu file text
            DateTime currentTime = DateTime.Now;
            long unixTimestamp = (long)(currentTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/" + unixTimestamp.ToString() + "list.txt");
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (TblAudio audio in updatedQuery)
                {
                    string audioName = audio.AudioName;
                    string transcript = audio.TblTranscript.Content;
                    writer.WriteLine(audioName + "|" + transcript + "|" + transcript);
                }
            }
            //Update file text vào tblTrain
            tblTrain.Folder = filePath;
            _context.Update(tblTrain);
            await _context.SaveChangesAsync();


            return RedirectToAction("Retrain", "TblModels", new { id = tblTrain.TblModelId }); // Replace 123 with the actual ID value
        }


        public async Task<IActionResult> CustomCreate(List<int> selectedItems)
        {
            DateTime currentTime = DateTime.Now;

            long unixTimestamp = (long)(currentTime - new DateTime(1970, 1, 1)).TotalMilliseconds;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/" + unixTimestamp.ToString() + "list.txt");

            IQueryable<TblAudio> listAudios = null;
            foreach (int id in selectedItems)
            {
                var audio = await _context.TblAudios.FindAsync(id);
                if (audio == null)
                {
                    continue;
                }
                if (listAudios == null)
                {
                    listAudios = from s in _context.TblAudios where s.Id == id select s;
                }
                else
                {
                    listAudios = listAudios.Union(from s in _context.TblAudios where s.Id == id select s);
                }
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
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (TblAudio audio in updatedQuery)
                {
                    string audioName = audio.AudioName;
                    string transcript = audio.TblTranscript.Content;
                    writer.WriteLine(audioName + "|" + transcript + "|" + transcript);
                }
            }

            foreach (TblAudio audio in updatedQuery)
            {
                if (audio.IsTrained != true)
                {
                    audio.IsTrained = true;
                    _context.Update(audio);
                }

            }
            await _context.SaveChangesAsync();

            TblTrain newTrain = new TblTrain();
            newTrain.Folder = filePath;
            _context.Add(newTrain);

            await _context.SaveChangesAsync();

            foreach (int id in selectedItems)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connString))
                    {
                        String queryInsert = "INSERT INTO [tacotron_tts].[dbo].[tblAudio_tblTrain] ([tblAudioId],[tblTrainId]) VALUES (" + id + "," + newTrain.Id + ")";
                        SqlCommand sqlCommand = new SqlCommand(queryInsert, connection);
                        connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        connection.Close();
                        Console.WriteLine("INSERT statement successfully executed.");

                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            return RedirectToAction("UpdatedTrainedTranscript", "tblTranscripts");
            // Your controller logic here
        }
        public async Task<IActionResult> ChooseTrainFileForModel()
        {
            return View(await _context.TblTrains.ToListAsync());
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

        public async Task<IActionResult> ChooseTrainForModel(int id)
        {
            int modelId = (int)(HttpContext.Session.GetInt32("modelId") ?? -1);
            if (modelId != -1)
            {
                var tblTrain = await _context.TblTrains.FindAsync(id);
                if (tblTrain == null)
                {
                    return NotFound();
                }
                tblTrain.TblModelId = modelId;
                _context.Update(tblTrain);
                await _context.SaveChangesAsync();

                String apiUrl = "http://" + getCurrentIp() + ":5000/run_tacotron2";
                string folder = tblTrain.Folder;
                string folderName = getFolderName(folder);

                HttpClient client = new HttpClient();
                string requestUrl = $"{apiUrl}?meta_file_train={folderName} & id_model={modelId}";

                Debug.WriteLine("API request: " + requestUrl);

                client.GetAsync(requestUrl);
                return RedirectToAction("Index", "TblModels");

            }
            return RedirectToAction("Index", "TblModels");
        }
        // GET: TblTrains/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TblTrains == null)
            {
                return NotFound();
            }

            var tblTrain = await _context.TblTrains.FindAsync(id);
            if (tblTrain == null)
            {
                return NotFound();
            }
            ViewData["TblModelId"] = new SelectList(_context.TblModels, "Id", "Id", tblTrain.TblModelId);
            return View(tblTrain);
        }

        // POST: TblTrains/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TblModelId,Folder")] TblTrain tblTrain)
        {
            if (id != tblTrain.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblTrain);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblTrainExists(tblTrain.Id))
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
            ViewData["TblModelId"] = new SelectList(_context.TblModels, "Id", "Id", tblTrain.TblModelId);
            return View(tblTrain);
        }

        // GET: TblTrains/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TblTrains == null)
            {
                return NotFound();
            }

            var tblTrain = await _context.TblTrains
                //.Include(t => t.TblModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblTrain == null)
            {
                return NotFound();
            }

            return View(tblTrain);
        }

        // POST: TblTrains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TblTrains == null)
            {
                return Problem("Entity set 'TacotronTtsContext.TblTrains'  is null.");
            }
            var tblTrain = await _context.TblTrains.FindAsync(id);
            if (tblTrain != null)
            {
                _context.TblTrains.Remove(tblTrain);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblTrainExists(int id)
        {
            return (_context.TblTrains?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
