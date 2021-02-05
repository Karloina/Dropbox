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
    //Zarządza plikami - które się pobierają/wrzucają oraz wątkami przydzielonymi do tych procesów
    class TransferManager
    {
        public static ConcurrentDictionary<string, TransferThread> ProcessingFiles { get; } = new ConcurrentDictionary<string, TransferThread>();
        private static readonly string ServerUrl = "https://localhost:44345";
        private static HttpClient _client = new HttpClient();
        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(5); // ile wątków chcemy dopuścić do danej sekcji jednocześnie - max 2 wątki mają dostęp do 1 sekcji
        private static int _delayTimeForTransfer = 5000;
        private static string _workingDirectory;
        private static string _login;
        private static Thread _t;
        private static bool active = false;

        //Rozpocznij nowy wątek
        public static void Start(string workingDirectory, string login)
        {
            _workingDirectory = workingDirectory;
            _login = login;
            active = true;
            _t = new Thread(Initiate); 
            _t.Start();
        }

        //Zatrzymaj wątki i nadaj procesującym plikom status Interrupted
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
                //wez liste lokalnych plikow
                string[] localFiles = Directory.GetFiles(_workingDirectory).Select(Path.GetFileName).ToArray();

                //wez liste plikow co sa na serwerze
                string[] serverFiles = Task.Run(async () =>
                {
                    var resp = await _client.GetAsync($"{ServerUrl}/files/{_login}");
                    var str = await resp.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<string[]>(str);
                    return result;
                }).Result;

                //na podstawie roznic wyznacz pliki do pobrania

                //nie ma takiej nazwy pliku lokalnie oraz nie ma go też w procesujących plikach (plik który wysyłamy/pobieramy nie zalicza się do plików 'do wrzutki' czy 'do pobrania')
                var toBeDownloaded =
                    serverFiles.Where(sf => !localFiles.Contains(sf) && !ProcessingFiles.Keys.Contains(sf));

                //oraz pliki do wrzucenia
                var toBeUploaded =
                    localFiles.Where(sf => !serverFiles.Contains(sf) && !ProcessingFiles.Keys.Contains(sf));

                //pobierz pliki do kolejki 'do pobrania' lokalnie
                foreach (var filename in toBeDownloaded)
                {
                    var dt = new DownloadThread(_semaphoreSlim, _delayTimeForTransfer, filename, _login, _workingDirectory, _client);
                    dt.Start(); //przydziel wątek do pobierania
                    ProcessingFiles.TryAdd(filename, dt);
                }

                //pobierz pliki do kolejki 'do wrzucenia' na serwer
                foreach (var filename in toBeUploaded)
                {
                    var ut = new UploadThread(_semaphoreSlim, _delayTimeForTransfer, filename, _login, _workingDirectory, _client);
                    ut.Start();
                    ProcessingFiles.TryAdd(filename, ut);
                }

                //poczekaj 1s - po co?
                Thread.Sleep(1000);     //1 s
            }
        }

        public static void RemoveFileTransfer(string procFile)
        {
            ProcessingFiles.Remove(procFile, out var empty);
        }
    }
}
