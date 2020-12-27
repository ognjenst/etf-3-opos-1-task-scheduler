using System;
using System.Threading;
using Xunit;

namespace Zadatak1.Test
{
    public class Tests
    {
        [Fact]
        public void AddingTasksTestNonPreemtive()
        {
            NonPriorityScheduler ts = new NonPriorityScheduler(3, 20);
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            Assert.True(ts.GetBCstate().Equals(6), "1 task has been added");
            ts.Start();
        }

        [Fact]
        public void ExecutingTasksTestNonPreemtive()
        {
            NonPriorityScheduler ts = new NonPriorityScheduler(3, 20);
            ts.AddTask(new Action(() => { Console.WriteLine("This is 1. live added task on thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(2000); }));
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            ts.AddTask(new Action(() => { Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); Console.WriteLine("Zavrsen"); }), 1000);
            ts.Start();
            Thread.Sleep(3000);
            Assert.True(ts.GetBCstate().Equals(0), "All tasks are done on time");
        }

        [Fact]
        public void AddingTasksTestPreemtive()
        {
            PriorityScheduler ts = new PriorityScheduler(3, 20);
            ts.AddTask(6, 10, new Action(() => { Console.WriteLine("This is task 5, with priority 6 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(6, 10, new Action(() => { Console.WriteLine("This is task 5, with priority 6 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(6, 10, new Action(() => { Console.WriteLine("This is task 5, with priority 6 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(6, 10, new Action(() => { Console.WriteLine("This is task 5, with priority 6 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(6, 10, new Action(() => { Console.WriteLine("This is task 5, with priority 6 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            ts.AddTask(6, 10, new Action(() => { Console.WriteLine("This is task 5, with priority 6 - executing on thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(1000); }));
            Assert.True(ts.getListState().Equals(6), "6 task has been added");
            ts.Start();
        }
    }
}
