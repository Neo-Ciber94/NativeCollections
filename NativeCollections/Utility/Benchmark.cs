using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NativeCollections.Utility
{
    public readonly struct BenchmarkResult
    {
        public TimeSpan Min { get; }
        public TimeSpan Max { get; }
        public TimeSpan Average { get; }

        public BenchmarkResult(TimeSpan min, TimeSpan max, TimeSpan average)
        {
            Min = min;
            Max = max;
            Average = average;
        }

        public override string ToString()
        {
            return $"Min: {Min}, Max: {Max}, Avg: {Average}";
        }
    }

    public static class Benchmark
    {
        public static void ComputeAndLog(string name, uint samples, Action action)
        {
            var result = Compute(samples, action);
            Console.WriteLine($"{name} >> {result}");
        }

        public static void ComputeGarbageAndLog(string name, uint samples, Action action)
        {
            var result = MeasureGarbage(samples, action);
            Console.WriteLine($"{name} >> {result} bytes");
        }

        public static BenchmarkResult Compute(Action action)
        {
            return Compute(1, action);
        }

        public static BenchmarkResult Compute(uint samples, Action action)
        {
            SortedSet<TimeSpan> times = new SortedSet<TimeSpan>();
            Stopwatch stopwatch = new Stopwatch();

            for (int i = 0; i < samples; i++)
            {
                stopwatch.Start();
                action();
                stopwatch.Stop();

                times.Add(stopwatch.Elapsed);
            }

            TimeSpan min = times.Min;
            TimeSpan max = times.Max;

            double totalMilliseconds = 0;
            foreach (var t in times)
            {
                totalMilliseconds += t.TotalMilliseconds;
            }

            TimeSpan avg = totalMilliseconds > 0 ? TimeSpan.FromMilliseconds(totalMilliseconds / times.Count) : default;
            return new BenchmarkResult(min, max, avg);
        }

        public static BenchmarkResult ComputeTime(Action action)
        {
            return ComputeTime(1, action);
        }

        public static BenchmarkResult ComputeTime(uint samples, Action action)
        {
            SortedSet<TimeSpan> times = new SortedSet<TimeSpan>();
            Stopwatch stopwatch = new Stopwatch();

            for (int i = 0; i < samples; i++)
            {
                stopwatch.Start();
                action();
                stopwatch.Stop();

                times.Add(stopwatch.Elapsed);
            }

            TimeSpan min = times.Min;
            TimeSpan max = times.Max;

            double totalMilliseconds = 0;
            foreach (var t in times)
            {
                totalMilliseconds += t.TotalMilliseconds;
            }

            TimeSpan avg = totalMilliseconds > 0 ? TimeSpan.FromMilliseconds(totalMilliseconds / times.Count) : default;
            return new BenchmarkResult(min, max, avg);
        }

        public static TimeSpan MeasureTime(Action action)
        {
            return MeasureTime(1, action);
        }

        public static TimeSpan MeasureTime(uint samples, Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (uint i = 0; i < samples; i++)
            {
                action();
            }

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static long MeasureGarbage(Action action)
        {
            return MeasureGarbage(1, action);
        }

        public static long MeasureGarbage(uint samples, Action action)
        {
            GC.WaitForPendingFinalizers();

            long startMemory = GC.GetTotalMemory(forceFullCollection: false);

            for (uint i = 0; i < samples; i++)
            {
                action();
            }

            long endMemory = GC.GetTotalMemory(forceFullCollection: false);
            return endMemory - startMemory;
        }
    }
}
