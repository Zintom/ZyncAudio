using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ZyncAudio
{
    public class WorkPump
    {
        private readonly ConcurrentQueue<IWorkItem> _workQueue = new();

        private readonly object _runLocker = new();

        private readonly object _workQueueFinishedLocker = new();

        private readonly ManualResetEvent _waitForWorkSignal = new(false);

        private volatile bool _stopPump;

        /// <summary>
        /// Runs the work pump on a new thread with the given <paramref name="threadPriority"/>.
        /// </summary>
        public void Run(ThreadPriority threadPriority, string threadName = "WorkPumpThread", bool isBackground = false)
        {
            new Thread(RunInternal)
            {
                Priority = threadPriority,
                Name = threadName,
                IsBackground = isBackground
            }.Start();
        }

        private void RunInternal()
        {
            if (!Monitor.TryEnter(_runLocker)) throw new InvalidOperationException("The message pump cannot be ran twice at the same time!");

            while (!_stopPump)
            {
                while (_workQueue.TryDequeue(out IWorkItem? work) && !_stopPump)
                {
                    work.Invoke();
                }

                lock (_workQueueFinishedLocker)
                {
                    // If the work queue is confirmed as Empty, then halt the loop until work becomes available.
                    if (_workQueue.IsEmpty && !_stopPump)
                    {
                        _waitForWorkSignal.Reset();
                    }
                }

                // Halt the loop until work becomes available.
                _waitForWorkSignal.WaitOne();
            }

            _stopPump = false;
            Monitor.Exit(_runLocker);
        }

        public void Add(IWorkItem workItem)
        {
            lock (_workQueueFinishedLocker)
            {
                _workQueue.Enqueue(workItem);

                // Inform loop that the work queue is not empty.
                _waitForWorkSignal.Set();
            }
        }

        /// <summary>
        /// Stops the pump from processing further work items, does not cancel the currently running work.
        /// </summary>
        public void Stop()
        {
            lock (_workQueueFinishedLocker)
            {
                _stopPump = true;
                _waitForWorkSignal.Set();
            }
        }

        /// <summary>
        /// Represents invokable work that does not return a value.
        /// </summary>
        public interface IWorkItem
        {
            void Invoke();
        }
    }
}
