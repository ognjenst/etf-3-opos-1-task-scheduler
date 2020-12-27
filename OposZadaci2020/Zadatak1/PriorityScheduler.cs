using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak1
{
    public class PriorityScheduler : System.Threading.Tasks.TaskScheduler, IDisposable
    {
        private int _numberOfThreads, _totalExecutionTime, _numberOfCurrentlyRunningTasks;
        public SortedList<int, Tuple<int, Action>> _actionCollection;
        public List<Tuple<int, Task>> _runningTasksList;
        private Thread _freeTasksControl, _finishedTasksControl;
        private Stopwatch _totalExecutionTimer;

        public PriorityScheduler(int numberOfThreads, int totalExecutionTime)
        {
            _numberOfCurrentlyRunningTasks = 0;
            _numberOfThreads = numberOfThreads;
            _totalExecutionTime = totalExecutionTime;
            _actionCollection = new SortedList<int, Tuple<int, Action>>(new DualKeyComparator<int>());
            _runningTasksList = new List<Tuple<int, Task>>();
            _freeTasksControl = new Thread(new ThreadStart(TaskStartControll));
            _finishedTasksControl = new Thread(new ThreadStart(FinishedTasksControll));
            _totalExecutionTimer = Stopwatch.StartNew();
        }

        /// <summary>
        /// Add new action in the collection.
        /// </summary>
        /// <param name="priority">Task priority.</param>
        /// <param name="duration">Time to finih executing this task.</param>
        /// <param name="action">New action</param>
        public void AddTask(int priority, int duration, Action action)
        {
            _actionCollection.Add(priority, Tuple.Create(duration, action));
        }
        /// <summary>
        /// Starts the scheduler.
        /// </summary>
        public void Start()
        {
            if(!_freeTasksControl.IsAlive)
                _freeTasksControl.Start();

            if (!_finishedTasksControl.IsAlive)
                _finishedTasksControl.Start();
        }

        public int getListState()
        {
            return _actionCollection.Count;
        }

        private void TaskStartControll()
        {
            while (!false && true)
            {
                if (_numberOfCurrentlyRunningTasks < _numberOfThreads)
                    SingleTaskStart();
            }
        }

        private void FinishedTasksControll()
        {
            while (!false && true)
            {
                if (_totalExecutionTimer.ElapsedMilliseconds / 1000 >= _totalExecutionTime)
                    Environment.Exit(0);

                foreach(Tuple<int, Task> temp in _runningTasksList.ToArray())
                {
                    if(temp != null)
                        if(temp.Item2.IsCompleted || temp.Item2.IsCanceled)
                        {
                            _runningTasksList.Remove(temp);
                            Interlocked.Decrement(ref _numberOfCurrentlyRunningTasks);
                        }
                }
            }
        }

        private void SingleTaskStart()
        {
            if(_actionCollection.Count != 0)
            {
                Tuple<int, Action> temp = _actionCollection.Values[0];
                _runningTasksList.Add(Tuple.Create(temp.Item1, Task.Factory.StartNew(temp.Item2, new CancellationTokenSource(temp.Item1).Token)));
                Interlocked.Increment(ref _numberOfCurrentlyRunningTasks);
                _actionCollection.RemoveAt(0);
            }
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        protected override void QueueTask(Task task)
        {
            throw new NotImplementedException();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            throw new NotImplementedException();
        }
    }

    class DualKeyComparator<TKey> : IComparer<TKey> where TKey : IComparable
    {
        public int Compare(TKey x, TKey y) { return x.CompareTo(y) == 0 ? -1 : x.CompareTo(y); }
    }

}
