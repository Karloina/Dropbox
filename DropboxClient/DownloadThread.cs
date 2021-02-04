using System;
using System.Threading;

namespace DropboxClient
{
    class DownloadThread : TransferThread
    {
        public DownloadThread(SemaphoreSlim s, int time, string filename, string login, string fileLocation) : base(s, time, filename, login, fileLocation)
        {
        }

        public override void ExecuteJob()
        {
            this.FileStatus = FileStatus.Downloading;
            // TODO: implement
        }
    }
}
