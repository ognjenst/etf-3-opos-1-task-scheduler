using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScosPresentation
{
    public class LaneWriter
    {
        readonly Mutex laneMutex = new Mutex();

        public int NumLanes => lanes.Length;

        readonly List<int>[] lanes;

        public LaneWriter(int numLanes)
        {
            laneMutex.WaitOne();
            lanes = new List<int>[numLanes];
            for (int i = 0; i < numLanes; ++i)
                lanes[i] = new List<int>();
            laneMutex.ReleaseMutex();
        }

        public void WriteToLane(int lane, int value) => lanes[lane].Add(value);

        public void PrintLanes()
        {
            laneMutex.WaitOne();
            Console.Clear();
            int maxLength = lanes.Max(x => x.Count);
            for (int i = 0; i < maxLength; ++i)
            {
                Console.Write($"T{i}:\t");
                for (int j = 0; j < NumLanes; ++j)
                    if (i < lanes[j].Count)
                        Console.Write($"{lanes[j][i]}\t");
                    else
                        Console.Write("_\t");
                Console.WriteLine();
            }
            laneMutex.ReleaseMutex();
        }
    }

    public static class ExtensionMethods
    {
        public static T ArgMin<T>(this IEnumerable<T> source, Func<T, int> selector)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            T minValue = default;
            int min = int.MaxValue;
            bool isAssigned = false;

            foreach (T item in source)
            {
                int value = selector(item);

                if (value < min || !isAssigned)
                {
                    isAssigned = true;
                    min = value;
                    minValue = item;
                }
            }

            return minValue;
        }
    }
}
