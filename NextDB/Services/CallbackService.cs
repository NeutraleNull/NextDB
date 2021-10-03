using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NextDB.Services
{
    public class CallbackService
    {
        private readonly unsafe delegate*unmanaged<string, string, string, int> _extensionCallback;
        private readonly ConcurrentQueue<(string, string)> _queue;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public unsafe CallbackService(delegate*unmanaged<string, string, string, int> extensionCallback)
        {
            _extensionCallback = extensionCallback;
            _cancellationTokenSource = new();
            _queue = new();
        }


        public void EnqueueEngineCallback(string function, string data)
        {
            _queue.Enqueue((function, data));
        }

        public CallbackService StartEngineCallback()
        {
            var thread = new Thread(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    while (!_queue.IsEmpty)
                    {
                        if (!_queue.TryDequeue(out (string function, string data) item)) continue;

                        var engineQueueId = CallEngine(ref item.function, ref item.data);

                        while (engineQueueId == -1)
                        {
                            Debug.WriteLine($"Callback to engine is full! Waiting...");
                            await Task.Delay(5, _cancellationTokenSource.Token);
                            engineQueueId = CallEngine(ref item.function, ref item.data);
                        }

                        Debug.WriteLine($"Data send back to engine: {item.Item1} {item.Item2}");
                    }
                }
            });
            thread.Start();
            return this;
        }

        private int CallEngine(ref string function, ref string data)
        {
            unsafe
            {
                return _extensionCallback(Constants.CallbackName, function, data);
            }
        }

        public void StopEngineCallback()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}