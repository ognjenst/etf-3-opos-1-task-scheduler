using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScosPresentation
{
    public static class RaceConditioner
    {
        const int numIncrements = 200000;

        volatile static int sharedData;

        public static void Demo()
        {
            const int numExperimentRepetitions = 5;

            for (int i = 0; i < numExperimentRepetitions; ++i)
            {
                Console.WriteLine($"Experiment repetition {i + 1}.");

                Demo1();
                Demo2();
                Demo3();
                Demo4();
                Demo5();
                Demo6();
                Demo7();

                Console.WriteLine();
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static void RunTwoTasks(Action action)
        {
            sharedData = 0;

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            Task t1 = Task.Factory.StartNew(action);
            Task t2 = Task.Factory.StartNew(action);

            t1.Wait();
            t2.Wait();

            stopwatch.Stop();

            Console.WriteLine($"{sharedData}: {stopwatch.ElapsedMilliseconds}");
        }

        private static void Demo1()
        {
            Console.Write("No synchronization\t");

            static void incrementData()
            {
                for (int i = 0; i < numIncrements; ++i)
                    ++sharedData;
            }

            RunTwoTasks(incrementData);
        }

        private static void Demo2()
        {
            Console.Write("Locker object\t\t");

            object locker = new object();

            void incrementData()
            {
                for (int i = 0; i < numIncrements; ++i)
                    lock (locker)
                        ++sharedData;
            }

            RunTwoTasks(incrementData);
        }

        private static void Demo3()
        {
            Console.Write("Mutex\t\t\t");

            Mutex mutex = new Mutex();

            void incrementData()
            {
                for (int i = 0; i < numIncrements; ++i)
                {
                    mutex.WaitOne();
                    ++sharedData;
                    mutex.ReleaseMutex();
                }
            }

            RunTwoTasks(incrementData);
        }

        private static void Demo4()
        {
            Console.Write("Semaphore\t\t");

            Semaphore semaphore = new Semaphore(1, 1);

            void incrementData()
            {
                for (int i = 0; i < numIncrements; ++i)
                {
                    semaphore.WaitOne();
                    ++sharedData;
                    semaphore.Release();
                }
            }

            RunTwoTasks(incrementData);
        }

        private static void Demo5()
        {
            Console.Write("Lightweight semaphore\t");

            SemaphoreSlim semaphore = new SemaphoreSlim(1);

            void incrementData()
            {
                for (int i = 0; i < numIncrements; ++i)
                {
                    semaphore.Wait();
                    ++sharedData;
                    semaphore.Release();
                }
            }

            RunTwoTasks(incrementData);
        }

        private static void Demo6()
        {
            Console.Write("Interlocked increment\t");

            static void incrementData()
            {
                for (int i = 0; i < numIncrements; ++i)
                    Interlocked.Increment(ref sharedData);
            }

            RunTwoTasks(incrementData);
        }

        private static void Demo7()
        {
            Console.Write("Monitor\t\t\t");

            object locker = new object();

            void incrementData()
            {
                for (int i = 0; i < numIncrements; ++i)
                {
                    Monitor.Enter(locker);
                    ++sharedData;
                    Monitor.Exit(locker);
                }
            }

            RunTwoTasks(incrementData);
        }
    }
}
