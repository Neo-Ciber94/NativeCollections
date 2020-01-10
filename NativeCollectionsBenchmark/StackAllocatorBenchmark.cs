using BenchmarkDotNet.Attributes;
using NativeCollections.Allocators;

namespace NativeCollectionsBenchmark
{
    [MemoryDiagnoser]
    public class StackAllocatorBenchmark
    {
        private StackAllocator stackAllocator;

        [Params(10, 100, 1000, 10000, 100000)]
        public int Bytes;

        [IterationSetup]
        public void Setup()
        {
            stackAllocator = new StackAllocator(1000_000);
        }

        [IterationCleanup]
        public void Close()
        {
            stackAllocator.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void ByteArray()
        {
            byte[] array = new byte[Bytes];
            for(int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    array[i] = (byte)i;
                }
            }
        }

        [Benchmark]
        unsafe public void Alloc()
        {
            byte* buffer = stackAllocator.Allocate<byte>(Bytes);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }

            stackAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void AllocUnitializated()
        {
            byte* buffer = (byte*)stackAllocator.Allocate(Bytes, sizeof(int), initMemory: false);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }

            stackAllocator.Free(buffer);
        }
    }
}
