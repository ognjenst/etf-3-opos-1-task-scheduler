using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Zadatak1
{
    class SynchronizedSortedActionsList
    {
        private readonly SortedList<int, Action> _actionsList = new SortedList<int, Action>(new DualKeyComparator<int>());
        private readonly Semaphore _pool = new Semaphore(0, int.MaxValue);
        private readonly object _lock = new object();

        public void Enqueue(int key, Action action)
        {
            lock (_lock)
            {
                _actionsList.Add(key, action);
                _pool.Release();
            }
        }

        public Action Dequeue()
        {
            if (_actionsList.Count == 0)
                return null;

            _pool.WaitOne();
            lock (_lock)
            {
                Action topTask = _actionsList.Values[0];
                _actionsList.RemoveAt(0);
                return topTask;
            }
        }
    }

    class DualKeyComparator<TKey> : IComparer<TKey> where TKey : IComparable
    {
        public int Compare(TKey x, TKey y) { return x.CompareTo(y) == 0 ? -1 : x.CompareTo(y); }
    }
}