using BenchmarkDotNet.Attributes;
using NativeCollections;

namespace NativeCollectionsBenchmark
{
    [ShortRunJob]
    [MemoryDiagnoser, MinColumn, MaxColumn]
    public class  NativeArrayVsArray
    {
        [Params(10, 100, 1000, 10000, 100000, 1000000)]
        public int ArraySize;

        [Benchmark]
        public void NativeArray()
        {
            NativeArray<int> array = new NativeArray<int>(ArraySize);
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }

            array.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void Array()
        {
            int[] array = new int[ArraySize];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }
        }
    }
}
