using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DropboxClient
{
    class UploadThread : TransferThread
    {
        public UploadThread(SemaphoreSlim s, int time, string filename, string login, string fileLocation, HttpClient httpClient) : base(s, time, filename, login, fileLocation, httpClient)
        {
        }

        public override void ExecuteJob()
        {
            FileStatus = FileStatus.Uploading;
            var gg = File.ReadAllBytes($@"{_fileLocation}\{Filename}");
            var byteArrayContent = new ByteArrayContent(gg);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(byteArrayContent, "file", Filename);
            var postResponse = Task.Run(async () => await _httpClient.PostAsync(@$"{_serverUrl}/files/{Login}", multipartContent)).Result;



            //albo to 
            // var postResponse = Task.Run(async () => await _httpClient.PostAsync("offers", new MultipartFormDataContent {
            //     { byteArrayContent, "csvFile", "filename" }
            // }));
         }
    }
}
