namespace Dropbox
{
    public abstract class TransferTask
    {
        public string Login { get; set; }
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public byte[] Bytes { get; set; }

        public abstract void Execute();
    }
}
