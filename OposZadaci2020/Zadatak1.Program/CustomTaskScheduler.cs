using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak1
{
    public class CustomTaskScheduler : System.Threading.Tasks.TaskScheduler, IDisposable
    {
        private int maxNumberOfTasks;
        private BlockingCollection<Task> taskCollection;
        
        private List<Thread> _threadPool;
        public CancellationTokenSource cts;
        private Thread _stopingThread;
        private static bool _stopingThreadFlag = false;
        public enum SchedulingType
        {
            Preemptive,
            NonPreemptive
        }

        public CustomTaskScheduler(int num, SchedulingType st)
        {
            maxNumberOfTasks = num;
            _threadPool = new List<Thread>(num);
            taskCollection = new BlockingCollection<Task>();
            cts = new CancellationTokenSource(3000);
            _stopingThread = new Thread(new ThreadStart(CheckStoppingFlag));
            for (int i = 0; i < maxNumberOfTasks; i++)
                _threadPool.Add(new Thread(new ThreadStart(Execute)));
        }

        public void AddTask(Action a)
        { 
            Task t = Task.Factory.StartNew(a, cts.Token, TaskCreationOptions.None, this);
        }

        private void CheckStoppingFlag()
        {
            while (true)
            {
                if (_stopingThreadFlag)
                {
                    cts.Cancel(); 
                    cts.Dispose();
                    
                }
            }
        }
        public void StartScheduler()
        {
            foreach (Thread t in _threadPool)
                if (!t.IsAlive)
                    t.Start();
        }

        public void StopScheduler()
        {
            _stopingThreadFlag = true;
        }

        public void getBCstate()
        {
            Console.WriteLine(taskCollection.Count);
        }


        private void Execute()
        {
            foreach (var task in taskCollection.GetConsumingEnumerable())
            {
                TryExecuteTask(task);
            }
        }

        public int MaxNumberOfTasks
        {
            get { return maxNumberOfTasks; }
            set { }
        }
        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            taskCollection.CompleteAdding();
            taskCollection.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return taskCollection.ToArray();
        }

        protected override void QueueTask(Task task)
        {
            if (task != null)
                taskCollection.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
        
    }
}
