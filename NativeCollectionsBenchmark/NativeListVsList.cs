using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using NativeCollections;
using NativeCollections.Allocators;

namespace NativeCollectionsBenchmark
{
    [ShortRunJob]
    [MemoryDiagnoser]
    public class NativeListVsList
    {
        [Params(10, 100, 1000, 10000, 100000, 1000000)]
        public int ListSize;

        private MemoryPoolAllocator allocator;

        [GlobalSetup]
        public void Setup()
        {
            allocator = new MemoryPoolAllocator(1000);
        }

        [GlobalCleanup]
        public void Close()
        {
            allocator.Dispose();
        }

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

        [Benchmark]
        public void NativeListFowardPoolAllocator()
        {
            NativeList<int> list = new NativeList<int>(ListSize, allocator);
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
