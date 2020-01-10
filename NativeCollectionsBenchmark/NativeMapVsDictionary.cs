using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using NativeCollections;

namespace NativeCollectionsBenchmark
{
    [ShortRunJob]
    [MemoryDiagnoser, MinColumn, MaxColumn]
    public class NativeMapVsDictionary
    {
        [Params(10, 100, 1000, 10000, 100000, 1000000)]
        public int MapSize;

        [Benchmark]
        public void NativeMap()
        {
            NativeMap<int, char> map = new NativeMap<int, char>(MapSize);
            for (int i = 0; i < map.Length; i++)
            {
                unchecked
                {
                    map.Add(i, (char)i);
                }
            }

            map.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void Dictionary()
        {
            Dictionary<int, char> map = new Dictionary<int, char>(MapSize);
            for (int i = 0; i < map.Count; i++)
            {
                unchecked
                {
                    map.Add(i, (char)i);
                }
            }

        }
    }
}
