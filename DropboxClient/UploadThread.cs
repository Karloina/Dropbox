using System;
using System.Threading;

namespace DropboxClient
{
    class UploadThread : TransferThread
    {
        public UploadThread(SemaphoreSlim s, int time, string filename, string login, string fileLocation) : base(s, time, filename, login, fileLocation)
        {
        }

        public override void ExecuteJob()
        {
            FileStatus = FileStatus.Uploading;
        }
    }
}
