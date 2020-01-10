using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using NativeCollections;

namespace NativeCollectionsBenchmark
{
    [ShortRunJob]
    [MemoryDiagnoser, MinColumn, MaxColumn]
    public class NativeListVsList
    {
        [Params(10, 100, 1000, 10000, 100000, 1000000)]
        public int ListSize;

        [Benchmark]
        public void NativeList()
        {
            NativeList<int> list = new NativeList<int>(ListSize);
            for (int i = 0; i < list.Length; i++)
            {
                list.Add(i);
            }

            list.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            List<int> list = new List<int>(ListSize);
            for (int i = 0; i < list.Count; i++)
            {
                list.Add(i);
            }
        }
    }
}
