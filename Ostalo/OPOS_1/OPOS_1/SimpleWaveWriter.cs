using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScosPresentation
{
    public static class SimpleWaveWriter
    {
        const int samplesPerSecond = 44100;
        const int totalSamples = samplesPerSecond * 60 * 10;

        private static List<short> GenerateData(int samples) => GenerateData(0, samples);

        private static List<short> GenerateData(int samplesStart, int samplesEnd)
        {
            const double ampl = 10000;
            const double freq = 220.0;

            List<short> data = new List<short>();
            for (int i = samplesStart; i < samplesEnd; i++)
            {
                double t = (double)i / samplesPerSecond;
                short s = (short)(ampl * (Math.Sin(t * freq * 2.0 * Math.PI)));
                data.Add(s);
            }

            return data;
        }

        private static void WriteData(BinaryWriter writer, List<short> data)
        {
            for (int i = 0; i < data.Count; ++i)
                writer.Write(data[i]);
        }

        private static void WriteHeader(BinaryWriter writer, int samples)
        {
            int RIFF = 0x46464952;
            int WAVE = 0x45564157;
            int formatChunkSize = 16;
            int headerSize = 8;
            int format = 0x20746D66;
            short formatType = 1;
            short tracks = 1;
            int samplesPerSecond = 44100;
            short bitsPerSample = 16;
            short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
            int bytesPerSecond = samplesPerSecond * frameSize;
            int waveSize = 4;
            int data = 0x61746164;
            int dataChunkSize = samples * frameSize;
            int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
            writer.Write(RIFF);
            writer.Write(fileSize);
            writer.Write(WAVE);
            writer.Write(format);
            writer.Write(formatChunkSize);
            writer.Write(formatType);
            writer.Write(tracks);
            writer.Write(samplesPerSecond);
            writer.Write(bytesPerSecond);
            writer.Write(frameSize);
            writer.Write(bitsPerSample);
            writer.Write(data);
            writer.Write(dataChunkSize);
        }

        public static void Demo1()
        {
            using FileStream stream = new FileStream("Test1.wav", FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream);

            WriteHeader(writer, totalSamples);

            List<short> data = GenerateData(totalSamples);
            WriteData(writer, data);
        }

        public static void Demo2()
        {
            using FileStream stream = new FileStream("Test2.wav", FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream);

            WriteHeader(writer, totalSamples);

            int numTasks = 10;
            List<short>[] results = new List<short>[numTasks];
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < numTasks; ++i)
            {
                int index = i;
                tasks.Add(Task.Factory.StartNew(() => results[index] = GenerateData(index * totalSamples / numTasks, (index + 1) * totalSamples / numTasks)));
            }

            Task.WhenAll(tasks).Wait();

            for (int i = 0; i < numTasks; ++i)
                WriteData(writer, results[i]);
        }

        public static void Demo3()
        {
            using FileStream stream = new FileStream("Test3.wav", FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream);

            WriteHeader(writer, totalSamples);

            int numTasks = Environment.ProcessorCount;
            List<short>[] results = new List<short>[numTasks];

            Parallel.For(0, numTasks, i => results[i] = GenerateData(i * totalSamples / numTasks, (i + 1) * totalSamples / numTasks));

            for (int i = 0; i < numTasks; ++i)
                WriteData(writer, results[i]);
        }

        public static void Demo4()
        {
            using FileStream stream = new FileStream("Test4.wav", FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream);

            WriteHeader(writer, totalSamples);

            int numTasks = 10;
            List<short>[] results = new List<short>[numTasks];
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < numTasks; ++i)
            {
                int index = i;
                tasks.Add(Task.Factory.StartNew(() => results[index] = GenerateData(index * totalSamples / numTasks, (index + 1) * totalSamples / numTasks)));
            }

            Task fileWriteTask = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < tasks.Count; ++i)
                {
                    tasks[i].Wait();
                    WriteData(writer, results[i]);
                }
            });

            fileWriteTask.Wait();
        }

        public static void Demo5()
        {
            using FileStream stream = new FileStream("Test5.wav", FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream);

            WriteHeader(writer, totalSamples);

            int numTasks = Environment.ProcessorCount;
            List<short>[] results = new List<short>[numTasks];

            List<Task> tasks = new List<Task>();

            int currentBuffer = 0;
            void writeToFile()
            {
                lock (results)
                    for (int i = currentBuffer; i < results.Length; ++i)
                        if (results[i] != null)
                            WriteData(writer, results[currentBuffer++]);
                        else
                            break;
            }

            Parallel.For(0, numTasks, i =>
            {
                results[i] = GenerateData(i * totalSamples / numTasks, (i + 1) * totalSamples / numTasks);
                tasks.Add(Task.Factory.StartNew(writeToFile));
            });

            Task.WhenAll(tasks).Wait();
        }

        public static void Demo()
        {
            const int experimentRepetitions = 5;

            for (int i = 0; i < experimentRepetitions; ++i)
            {
                Console.WriteLine($"Experiment repetition {i + 1}.");

                Action[] demoFunctions = new Action[] { Demo1, Demo2, Demo3, Demo4, Demo5 };

                for (int j = 0; j < demoFunctions.Length; ++j)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    demoFunctions[j]();
                    stopwatch.Stop();
                    Console.WriteLine($"Demo{j + 1}: {stopwatch.Elapsed}");
                }

                Console.WriteLine();
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
