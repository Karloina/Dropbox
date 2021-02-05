using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DropboxClient
{
    class DownloadThread : TransferThread
    {
        public DownloadThread(SemaphoreSlim s, int time, string filename, string login, string fileLocation, HttpClient httpClient) : base(s, time, filename, login, fileLocation, httpClient)
        {
        }

        public override void ExecuteJob()
        {
            this.FileStatus = FileStatus.Downloading;

            var bytes = Task.Run(async () =>
                await _httpClient.GetByteArrayAsync(@$"{_serverUrl}/files/{Login}/{Filename}")).Result;

            File.WriteAllBytes($@"{_fileLocation}\{Filename}", bytes);
        }
    }
}
