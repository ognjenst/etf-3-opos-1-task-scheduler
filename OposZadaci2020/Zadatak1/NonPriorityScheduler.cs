using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak1
{
    public class NonPriorityScheduler : System.Threading.Tasks.TaskScheduler, IDisposable
    {
        private int _numberOfThreads;
        private BlockingCollection<Task> _taskCollection;
        private List<Thread> _threadPool;
        private Thread _controlThread;
        private static bool _stopingThreadFlag = false;
        private int _totalExecutionTime;
        private static Stopwatch _totalExecutionTimer;

        public NonPriorityScheduler(int numberOfThreads, int totalTimeToExecute)
        {
            _totalExecutionTimer = Stopwatch.StartNew();
            _numberOfThreads = numberOfThreads;
            _totalExecutionTime = totalTimeToExecute;
            _threadPool = new List<Thread>(numberOfThreads);
            _taskCollection = new BlockingCollection<Task>();
            _controlThread = new Thread(new ThreadStart(ControllFunction));
            for (int i = 0; i < _numberOfThreads; i++)
                _threadPool.Add(new Thread(new ThreadStart(Execute)));
        }

        /// <summary>
        /// Add new action in the collection.
        /// </summary>
        /// <param name="action">New action.</param>
        /// <param name="timeToExecute">Time to finih executing this task.</param>
        public void AddTask(Action action, int timeToExecute = 10000)
        {
            Task.Factory.StartNew(action, new CancellationTokenSource().Token, TaskCreationOptions.None, this);
        }

        private void ControllFunction()
        {
            while (true)
            {
                if (_totalExecutionTimer.ElapsedMilliseconds / 1000 >= _totalExecutionTime)
                    Environment.Exit(0);
                if (_stopingThreadFlag)
                {
                    Dispose();
                    Environment.Exit(0);
                }
            }
        }
        /// <summary>
        /// Starts the scheduler.
        /// </summary>
        public void Start()
        {
            foreach (Thread t in _threadPool)
                if (!t.IsAlive)
                    t.Start();
            _controlThread.Start();
        }

        /// <summary>
        /// Stops the scheduler.
        /// </summary>
        public void Stop()
        {
            _stopingThreadFlag = true;
        }

        /// <summary>
        /// Current number of tasks stored in collection.
        /// </summary>
        /// <returns></returns>
        public int GetBCstate()
        {
            return _taskCollection.Count;
        }


        private void Execute()
        {
            foreach (var task in _taskCollection.GetConsumingEnumerable())
            {
                TryExecuteTask(task);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            _taskCollection.CompleteAdding();
            _taskCollection.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// IEnumerable override
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _taskCollection.ToArray();
        }

        /// <summary>
        /// QueueTask override
        /// </summary>
        /// <param name="task">New task</param>
        protected override void QueueTask(Task task)
        {
            if (task != null)
                _taskCollection.Add(task);
        }

        /// <summary>
        /// TryExecuteTaskInline override. Prevent inline execution
        /// </summary>
        /// <param name="task">task</param>
        /// <param name="taskWasPreviouslyQueued">Was the task previously queued.</param>
        /// <returns></returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

    }
}
