#pragma warning disable CS0618 // Thread.Resume and Thread.Suspend are deprecated
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScosPresentation
{
    public static class SimpleScheduler
    {
        const int pauseInMilliseconds = 1000;

        public static void Demo()
        {
            const int maxDurationInSeconds = 2;

            Thread t1 = new Thread(() => WriteIndefinitely("Thread 1"));
            Thread t2 = new Thread(() => WriteIndefinitely("Thread 2"));

            t1.Start();
            t2.Start();

            bool currentFirstThread = true;
            for (int i = 0; i < maxDurationInSeconds; ++i)
            {
                if (currentFirstThread)
                    SuspendAndResume(t1, t2);
                else
                    SuspendAndResume(t2, t1);
                Thread.Sleep(pauseInMilliseconds);
                currentFirstThread = !currentFirstThread;
            }

            AbortOrJoin(t1);
            AbortOrJoin(t2);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        static void WriteIndefinitely(string message)
        {
            const int pauseInMilliseconds = 100;
            while (true)
            {
                Console.WriteLine(message);
                Thread.Sleep(pauseInMilliseconds);
            }
        }

        static void SuspendAndResume(Thread t1, Thread t2)
        {
            if ((t1.ThreadState & ThreadState.Suspended) == ThreadState.Suspended)
                t1.Resume();
            if ((t2.ThreadState & ThreadState.Running) == ThreadState.Running)
                t2.Suspend();
        }

        static void AbortOrJoin(Thread t)
        {
            if (t.ThreadState == ThreadState.Stopped)
                t.Join();
            else
            {
                if ((t.ThreadState & ThreadState.Suspended) == ThreadState.Suspended)
                    t.Resume();
                Thread.Sleep(pauseInMilliseconds);
                t.Abort();
            }
        }
    }
}
#pragma warning restore CS0618