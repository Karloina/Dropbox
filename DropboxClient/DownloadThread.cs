using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DropboxClient
{
    class DownloadThread : TransferThread
    {
        public DownloadThread(SemaphoreSlim s, int time, string filename, string login, Action<string> finishedReceiver, string fileLocation) : base(s, time, filename, login, finishedReceiver, fileLocation)
        {
        }

        public override void ExecuteJob()
        {
            this.FileStatus = FileStatus.Downloading;
            // TODO: implement
        }
    }
}
