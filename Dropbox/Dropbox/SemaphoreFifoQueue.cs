using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Dropbox
{
    public class SemaphoreFifoQueue
    {
        private SemaphoreSlim semaphore;
        private ConcurrentQueue<TaskCompletionSource<bool>> queue =
            new ConcurrentQueue<TaskCompletionSource<bool>>();
        public SemaphoreFifoQueue(int initialCount)
        {
            semaphore = new SemaphoreSlim(initialCount);
        }
       
        public void Wait()
        {
            WaitAsync().Wait();
        }

        public Task WaitAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            queue.Enqueue(tcs);
            //za każdym razem jak dodajemy do kolejki task to też rejestrujemy do metody WaitAsync zdjęcie jednego elementu z kolejki
            semaphore.WaitAsync().ContinueWith(t =>
            {
                TaskCompletionSource<bool> popped;
                if (queue.TryDequeue(out popped))
                    popped.SetResult(true);
            });
            return tcs.Task;
        }
        public void Release()
        {
            semaphore.Release();
        }
    }
}
