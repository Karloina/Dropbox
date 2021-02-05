using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Dropbox
{
    public static class FileManager
    {
        public static ConcurrentDictionary<Guid, TransferTask> tasks = new ConcurrentDictionary<Guid, TransferTask>();
        private static SemaphoreFifoQueue _semaphoreFifoQueue = new SemaphoreFifoQueue(1);

        public static byte[] DownloadFile(string login, string fileName, string location)
        {
            var task = new DownloadTask()
            {
                Login = login,
                FileLocation = location,
                FileName = fileName
            };
            
            tasks.TryAdd(task.Id, task);
            _semaphoreFifoQueue.Wait();

            task.Execute();

            _semaphoreFifoQueue.Release();
            return task.Bytes;
        }

        public static void UploadFile(string login, string fileName, IFormFile formFile, string location)
        {
            var task = new UploadTask()
            {
                Login = login,
                FileLocation = location,
                FileName = fileName,
                FormFile = formFile
            };

            tasks.TryAdd(task.Id, task);
            _semaphoreFifoQueue.Wait();

            task.Execute();

            _semaphoreFifoQueue.Release();
            //tasks.TryRemove(task.Id, out var empty);
        }

    }
}
