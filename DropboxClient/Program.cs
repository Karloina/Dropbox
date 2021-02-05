using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace DropboxClient
{
    class Program
    {
        private static readonly string ServerUrl = "https://localhost:44345";
        private static string _login;
        private static string _dir;
        private static HttpClient _client = new HttpClient();
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
               return;
            }

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            _login = args[0];
            _dir = args[1];
            var resp = await _client.GetAsync($"{ServerUrl}/authenticate/{_login}");

            if (!resp.IsSuccessStatusCode)
            {
                Console.WriteLine(await resp.Content.ReadAsStringAsync());
                return;
            }

            //usaw skanowanie folderu na konkretny katalog i użytkownika - zobacz co do pobrania i co do wrzucenia
            TransferManager.Start(_dir, _login);

            while (true)
            {
                Console.Clear();
                PrintLoginDirectoryInfo(_login, _dir);
                Console.WriteLine("Files in progress:");
                foreach (var fileTranfer in TransferManager.ProcessingFiles.Values)
                {
                    Console.WriteLine($"{fileTranfer.Filename} : {fileTranfer.FileStatus}");
                }

                Console.WriteLine("Local Files:");
                ListFilesInDirectory(_dir);

                Thread.Sleep(500);     // 0,5 s
            }

        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            TransferManager.Stop();
            Console.WriteLine("logging out");
            var task = Task.Run(async () => await _client.DeleteAsync($"{ServerUrl}/authenticate/{_login}"));
            task.Wait();
        }

        static void PrintLoginDirectoryInfo(string login, string directory)
        {
            Console.WriteLine($"Login: {login}");
            Console.WriteLine($"Directory: {directory}");
        }

        static void ListFilesInDirectory(string workingDirectory)
        {
            string[] files = Directory.GetFiles(workingDirectory);
            foreach (string file in files)
                Console.WriteLine(Path.GetFileName(file));
        }
    }
}
