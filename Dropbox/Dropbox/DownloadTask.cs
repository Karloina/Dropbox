namespace Dropbox
{
    public class DownloadTask : TransferTask
    {
        public byte[] Bytes { get; set; }

        public override void DelayedJob()
        {
            Bytes = System.IO.File.ReadAllBytes($@"{FileLocation}\{Login}\{FileName}");
        }
    }
}
