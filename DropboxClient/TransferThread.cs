using System.Net.Http;
using System.Threading;

namespace DropboxClient
{
    abstract class TransferThread
    {
        private SemaphoreSlim _s;
        private int _time;
        public string Filename { get; }
        public string Login { get; }
        public FileStatus FileStatus { get; set; }
        protected string _fileLocation;
        private Thread _t;
        protected HttpClient _httpClient;
        protected static readonly string _serverUrl = "https://localhost:44345";


        protected TransferThread(SemaphoreSlim s, int time, string filename, string login, string fileLocation, HttpClient httpClient)
        {
            _s = s;
            _time = time;
            Filename = filename;
            Login = login;
            _fileLocation = fileLocation;
            _t = new Thread(Job);
            FileStatus = FileStatus.Preparing;
            _httpClient = httpClient;
        }

        public void Start()
        {
            _t.Start();
        }

        public void Job()
        {
            _s.Wait();
            ExecuteJob();
            _s.Release();
            FileStatus = FileStatus.Finished;
            TransferManager.RemoveFileTransfer(Filename);
        }

        public abstract void ExecuteJob();

        public void Cancel()
        {
            _t.Interrupt();
            FileStatus = FileStatus.Interrupted;
        }
    }
}
