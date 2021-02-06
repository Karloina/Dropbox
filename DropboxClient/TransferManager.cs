using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DropboxClient
{
    class TransferManager
    {
        public static ConcurrentDictionary<string, TransferThread> ProcessingFiles { get; } = new ConcurrentDictionary<string, TransferThread>();
        private static readonly string ServerUrl = "https://localhost:44345";
        private static HttpClient _client = new HttpClient();
        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(5);
        private static int _delayTimeForTransfer = 5000;
        private static string _workingDirectory;
        private static string _login;
        private static Thread _t;
        private static bool active = false;

        public static void Start(string workingDirectory, string login)
        {
            _workingDirectory = workingDirectory;
            _login = login;
            active = true;
            _t = new Thread(Initiate); 
            _t.Start();
        }

        public static void Stop()
        {
            active = false;
            foreach (var fileTransfer in ProcessingFiles.Values)
            {
                fileTransfer.Cancel();
            }
        }

        private static void Initiate()
        {
            while (active)
            {
                string[] localFiles = Directory.GetFiles(_workingDirectory).Select(Path.GetFileName).ToArray();
                string[] serverFiles = Task.Run(async () =>
                {
                    var resp = await _client.GetAsync($"{ServerUrl}/files/{_login}");
                    var str = await resp.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<string[]>(str);
                    return result;
                }).Result;

                var toBeDownloaded =
                    serverFiles.Where(sf => !localFiles.Contains(sf) && !ProcessingFiles.Keys.Contains(sf));

                var toBeUploaded =
                    localFiles.Where(sf => !serverFiles.Contains(sf) && !ProcessingFiles.Keys.Contains(sf));

                foreach (var filename in toBeDownloaded)
                {
                    var dt = new DownloadThread(_semaphoreSlim, _delayTimeForTransfer, filename, _login, _workingDirectory, _client);
                    dt.Start();
                    ProcessingFiles.TryAdd(filename, dt);
                }

                foreach (var filename in toBeUploaded)
                {
                    var ut = new UploadThread(_semaphoreSlim, _delayTimeForTransfer, filename, _login, _workingDirectory, _client);
                    ut.Start();
                    ProcessingFiles.TryAdd(filename, ut);
                }

                Thread.Sleep(1000);     //1 s
            }
        }

        public static void RemoveFileTransfer(string procFile)
        {
            ProcessingFiles.Remove(procFile, out var empty);
        }
    }
}
