#pragma warning disable CA1034, CA1031 // Nested types and try-catch blocks
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScosPresentation
{
    public class NaiveTaskScheduler
    {
        public delegate void SchedulableTask(DataReporter dataReporter);

        public class DataReporter
        {
            public int CurrentThreadId { get; private set; }
            public bool IsCancelled { get; private set; }

            public DataReporter(int currentThreadId) => CurrentThreadId = currentThreadId;

            public void Cancel() => IsCancelled = true;
        }

        readonly List<(SchedulableTask task, int maxDuration, int threadId)> pendingTasks = new List<(SchedulableTask, int, int)>();

        readonly (SchedulableTask task, DataReporter dataReporter, Task executingTask, Task taskWithCallback, CancellationTokenSource cancellationTokenSource)?[] executingTasks;

        public int MaxParallelTasks => executingTasks.Length;

        public int CurrentTaskCount => executingTasks.Count(x => x.HasValue);

        public NaiveTaskScheduler(int maxParallelTasks) => executingTasks = new (SchedulableTask, DataReporter, Task, Task, CancellationTokenSource)?[maxParallelTasks];

        private void SchedulePendingTasks()
        {
            AbortTasksOverQuota();
            ScheduleTasksOnAvailableThreads();
        }

        private void AbortTasksOverQuota()
        {
            for (int i = 0; i < MaxParallelTasks; ++i)
            {
                if (executingTasks[i].HasValue)
                {
                    (SchedulableTask task, DataReporter dataReporter, Task executingTask, _, CancellationTokenSource cancellationTokenSource) = executingTasks[i].Value;
                    if (cancellationTokenSource.IsCancellationRequested || executingTask.IsCanceled || executingTask.IsCompleted)
                    {
                        executingTasks[i] = null;
                        dataReporter.Cancel();
                        pendingTasks.RemoveAll(x => x.task == task);
                    }
                }
            }
        }

        private void ScheduleTasksOnAvailableThreads()
        {
            int[] availableThreads = executingTasks.Select((value, i) => (value, i)).Where(x => !x.value.HasValue).Select(x => x.i).ToArray();
            foreach (int freeThread in availableThreads)
            {
                if (pendingTasks.Any(x => x.threadId == freeThread))
                {
                    (SchedulableTask task, int maxDuration, int threadId) = pendingTasks.First(x => x.threadId == freeThread);
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(maxDuration);
                    DataReporter dataReporter = new DataReporter(threadId);
                    Task scheduledTask = Task.Factory.StartNew(() => task(dataReporter), cancellationTokenSource.Token);
                    Task taskWithCallback = Task.Factory.StartNew(() =>
                    {
                        Task.Delay(maxDuration).Wait();
                        dataReporter.Cancel();
                        cancellationTokenSource.Cancel();
                        scheduledTask.Wait();
                        SchedulePendingTasks();
                    });
                    executingTasks[freeThread] = (task, dataReporter, scheduledTask, taskWithCallback, cancellationTokenSource);
                }
            }
        }

        public void ScheduleTask(SchedulableTask task, int maxDuration)
        {
            int threadWithMinimumWork;
            int[] freeThreads = Enumerable.Range(0, MaxParallelTasks).Except(pendingTasks.Select(x => x.threadId).Distinct()).ToArray();
            if (freeThreads.Length > 0)
                threadWithMinimumWork = freeThreads[0];
            else
                threadWithMinimumWork = pendingTasks.GroupBy(x => x.threadId).Select(x => (threadId: x.Key, totalLength: x.Sum(y => y.maxDuration))).ArgMin(x => x.totalLength).threadId;
            pendingTasks.Add((task, maxDuration, threadWithMinimumWork));

            SchedulePendingTasks();
        }

        public static void Demo()
        {
            const int numThreads = 5;
            const int numTasks = 15;
            const int oneSecondDelayInMilliseconds = 1000;

            NaiveTaskScheduler naiveTaskScheduler = new NaiveTaskScheduler(numThreads);
            LaneWriter laneWriter = new LaneWriter(numThreads);

            void printFunction(DataReporter dataReporter, int value, int duration)
            {
                for (int i = 0; i < duration; ++i)
                {
                    if (dataReporter.IsCancelled)
                        break;
                    laneWriter.WriteToLane(dataReporter.CurrentThreadId, value);
                    laneWriter.PrintLanes();
                    Task.Delay(oneSecondDelayInMilliseconds).Wait();
                }
            }

            SchedulableTask[] tasks = new SchedulableTask[numTasks];
            for (int i = 0; i < numTasks; ++i)
            {
                int lane = i;
                tasks[i] = x => printFunction(x, lane, i > 10 ? 4 : 10);
                naiveTaskScheduler.ScheduleTask(tasks[i], 5 * oneSecondDelayInMilliseconds);
            }

            while (naiveTaskScheduler.CurrentTaskCount > 0)
                Task.Delay(oneSecondDelayInMilliseconds).Wait();

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
#pragma warning restore CA1034, CA1031