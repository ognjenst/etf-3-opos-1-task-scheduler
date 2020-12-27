using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak1
{
    public class SimpleScheduler : System.Threading.Tasks.TaskScheduler
    {
        private readonly SynchronizedSortedActionsList _prioritizedActions = new SynchronizedSortedActionsList();
        private readonly SchedulingType _schedulingType;
        private static int _availableThreads;
        private readonly List<Task> _runningTasks = new List<Task>();
        private static int _currentlyRunning = 0;
        private readonly object _lock = new object();
        private readonly Thread _countingThread;
        private Thread _nonPreemptiveSchedulingThread;
        private Thread _preemptiveSchedulingThread;


        public SimpleScheduler(SchedulingType type, int threadCount)
        {
            if (threadCount < 1)
                throw new ArgumentOutOfRangeException(nameof(threadCount), "Must be at least 1");

            _schedulingType = type;
            _availableThreads = threadCount;

            // Counting Tasks Thread
            // Task.Factory.StartNew(new Action(GetRunningTasksCount));
            _countingThread = new Thread(new ThreadStart(GetRunningTasksCount));
            _countingThread.Start();

            _nonPreemptiveSchedulingThread = new Thread(new ThreadStart(ExecuteTasksNoPreemption));
        }

        public void QueueAction(Action action, int priority)
        {
            _prioritizedActions.Enqueue(priority, action);
            QueueTask(new Task(action));
        }

        public void Schedule()
        {
            if (_schedulingType.Equals(SchedulingType.NonPreemptive))
                _nonPreemptiveSchedulingThread.Start();
            // Task.Factory.StartNew(new Action(ExecuteTasksNoPreemption));
            else if (_schedulingType.Equals(SchedulingType.Preemptive)) ;
            // ExecuteTasksPreemption();
        }

        private void GetRunningTasksCount()
        {
            while (true)
                foreach (Task t in _runningTasks.ToArray())
                    if (t.IsCompleted)
                    {
                        lock (_lock)
                            _runningTasks.Remove(t);
                        Interlocked.Decrement(ref _currentlyRunning);
                    }
        }

        private void ExecuteTasksNoPreemption()
        {
            for (int i = 0; i < _availableThreads; i++)
            {
                Action action = _prioritizedActions.Dequeue();

                if (action == null)
                    continue;

                lock (_lock)
                    _runningTasks.Add(Task.Factory.StartNew(action));
                Interlocked.Increment(ref _currentlyRunning);
            }

            while (true)
                if (_currentlyRunning < _availableThreads)
                {
                    for (int i = 0; i < _availableThreads - _currentlyRunning; i++)
                    {
                        Action action = _prioritizedActions.Dequeue();
                        if (action == null)
                            continue;
                        lock (_lock)
                            _runningTasks.Add(Task.Factory.StartNew(action));
                        Interlocked.Increment(ref _currentlyRunning);
                    }
                }
        }

        // Overrides from TaskScheduler
        protected override void QueueTask(Task task)
        {
            return;
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }
    }

    public enum SchedulingType
    {
        Preemptive,
        NonPreemptive
    }

    public class ActionMetadata
    {
        int _priority;
        int _duration;
        List<String> _resources;

        public ActionMetadata(int priority, int duration, List<String> resources = null)
        {
            _priority = priority;
            _duration = duration;
            _resources = resources;
        }
    }
}
