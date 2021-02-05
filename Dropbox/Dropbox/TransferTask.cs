using System;
using System.Threading;

namespace Dropbox
{
    public abstract class TransferTask
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Login { get; set; }
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public TransferStatus TransferStatus { get; set; } = TransferStatus.Pending;

        public void Execute()
        {
            TransferStatus = TransferStatus.InProgress;
            Thread.Sleep(5000);
            DelayedJob();
            TransferStatus = TransferStatus.Finished;
        }

        public abstract void DelayedJob();
    }

    public enum TransferStatus
    {
        Pending, 
        InProgress,
        Finished
    }
}
