﻿using System;
using System.Threading;

namespace DropboxClient
{
    //1 watek
    abstract class TransferThread
    {
        private SemaphoreSlim _s;
        private int _time;
        private Action<string> _action;
        public string Filename { get; private set; }
        public string Login { get; private set; }
        public FileStatus FileStatus { get; set; }
        private string _fileLocation;

        private Thread _t;
        public TransferThread(SemaphoreSlim s, int time, string filename, string login, Action<string> finishedReceiver, string fileLocation)
        {
            _s = s;
            _time = time;
            _action = finishedReceiver;
            Filename = filename;
            Login = login;
            _fileLocation = fileLocation;
            _t = new Thread(DelayedJob);
            FileStatus = FileStatus.Preparing;
        }

        public void Start()
        {
            _t.Start();
        }

        public void DelayedJob()
        {
            _s.Wait();
            ExecuteJob();
            Thread.Sleep(_time);
            _s.Release();
            FileStatus = FileStatus.Finished;
            _action(Filename);
        }
        public abstract void ExecuteJob();

        public void Cancel()
        {
            _t.Interrupt();
            FileStatus = FileStatus.Interrupted;
        }
    }
}
