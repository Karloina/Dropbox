using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace Dropbox
{
    public static class FileManager
    {
        private static ConcurrentQueue<TransferTask> waitingQueue = new ConcurrentQueue<TransferTask>();
        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(5);

        public static void TryExecute()
        {
            _semaphoreSlim.Wait();
            waitingQueue.TryPeek(out var task);
            task.Execute();
            _semaphoreSlim.Release();
        }

        public static byte[] DownloadFile(string login, string fileName)
        {
            throw new NotImplementedException();
        }

        public static void UploadFile(string login, string fileName, Stream stream)
        {
            //kontrolowana przez semafor
        }



    }
}
