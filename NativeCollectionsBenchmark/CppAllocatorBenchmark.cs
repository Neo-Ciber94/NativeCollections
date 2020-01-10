using BenchmarkDotNet.Attributes;
using NativeCollections.Allocators;

namespace NativeCollectionsBenchmark
{
    [MemoryDiagnoser]
    public class CppAllocatorBenchmark
    {
        private DefaultCppAllocator cppAllocator;

        [Params(10, 100, 1000, 10000, 100000)]
        public int Bytes;

        [IterationSetup]
        public void Setup()
        {
            cppAllocator = DefaultCppAllocator.Instance;
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
            byte* buffer = cppAllocator.Allocate<byte>(Bytes);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }

            cppAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void AllocUnitializated()
        {
            byte* buffer = (byte*)cppAllocator.Allocate(Bytes, sizeof(int), initMemory: false);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }

            cppAllocator.Free(buffer);
        }
    }
}
