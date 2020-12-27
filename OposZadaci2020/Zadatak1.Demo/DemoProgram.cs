using System;
using System.Threading;

namespace Zadatak1.Demo
{
    class DemoProgram
    {
        static void Main(string[] args)
        {
            /* START OF NON PRIORITY SCHEDULER */

            //NonPriorityScheduler ts = new NonPriorityScheduler(4, 25);
            //ts.AddTask(new Action(() => { Console.WriteLine("This is task 1 executing on thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(5000); }));
            //ts.AddTask(new Action(() => { Console.WriteLine("This is task 2 executing on thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(2000); }));
            //ts.AddTask(new Action(() => { Console.WriteLine("This is task 3 executing on thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(3000); }));
            //ts.AddTask(new Action(() => { Console.WriteLine("This is task 4 executing on thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(4000); }));
            //ts.AddTask(new Action(() => { Console.WriteLine("This is task 5 executing on thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(5000); }));
            //ts.AddTask(new Action(() => { Console.WriteLine("This is task 6 executing on thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(5000); }));
            //ts.Start();
            //ts.AddTask(new Action(() => { Console.WriteLine("This is 1. live added task on thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(2000); }));
            //ts.AddTask(new Action(() => { Console.WriteLine("This is 2. live added task on thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(2000); }));
            //ts.AddTask(new Action(() => { Console.WriteLine("This is 3. live added task on thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(2000); }));

            /* END OF NON PRIORITY SCHEDULER */
            /* ----------------------------- */
            /*  START OF PRIORITY SCHEDULER  */

            PriorityScheduler ts = new PriorityScheduler(3, 50);
            ts.AddTask(1, 10, new Action(() => { Console.WriteLine("This is task 1, with priority 1 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(2, 10, new Action(() => { Console.WriteLine("This is task 2, with priority 2 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(0, 10, new Action(() => { Console.WriteLine("This is task 3, with priority 0 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(0, 10, new Action(() => { Console.WriteLine("This is task 4, with priority 0 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(6, 10, new Action(() => { Console.WriteLine("This is task 5, with priority 6 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(4, 10, new Action(() => { Console.WriteLine("This is task 6, with priority 4 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.Start();
            ts.AddTask(1, 10, new Action(() => { Console.WriteLine("++ Real time 1, with priority 1 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(2, 10, new Action(() => { Console.WriteLine("++ Real time 2, with priority 2 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(0, 10, new Action(() => { Console.WriteLine("++ Real time 3, with priority 0 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(1, 10, new Action(() => { Console.WriteLine("++ Real time 4, with priority 1 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(5, 10, new Action(() => { Console.WriteLine("++ Real time 5, with priority 5 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            Thread.Sleep(5000);

            /* END OF PRIORITY SCHEDULER */
        }
    }
}
