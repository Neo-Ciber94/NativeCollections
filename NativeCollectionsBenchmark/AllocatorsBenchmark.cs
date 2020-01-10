using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using NativeCollections.Allocators;
using NativeCollections.Allocators.Internal;

namespace NativeCollectionsBenchmark
{
    [MemoryDiagnoser]
    [ShortRunJob]
    public class AllocatorsBenchmark
    {
        private DefaultHeapAllocator heapAllocator;
        private DefaultLocalAllocator localAllocator;
        private DefaultCppAllocator cAllocator;
        private ArenaAllocator arenaAllocator;
        private StackAllocator stackAllocator;
        private FixedMemoryPoolAllocator poolAllocator;

        [Params(10, 100, 1000, 10000)]
        public int Bytes;

        [IterationSetup]
        public void Setup()
        {
            const int AllocatorSize = 1000_000;
            var defaultAlloc = Allocator.Default;
            if(defaultAlloc == null)
            {
                throw new Exception("Null default allocator");
            }

            heapAllocator = DefaultHeapAllocator.Instance;
            localAllocator = DefaultLocalAllocator.Instance;
            cAllocator = DefaultCppAllocator.Instance;
            arenaAllocator = new ArenaAllocator(AllocatorSize);
            stackAllocator = new StackAllocator(AllocatorSize);
            poolAllocator = new FixedMemoryPoolAllocator(10, AllocatorSize / 2);
        }

        [IterationCleanup]
        public void Close()
        {
            arenaAllocator.Dispose();
            stackAllocator.Dispose();
            poolAllocator.Dispose();
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
        unsafe public void HeapAllocatorAlloc()
        {
            byte* buffer = heapAllocator.Allocate<byte>(Bytes);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }
            heapAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void HeapAllocatorAllocUnitializated()
        {
            byte* buffer = (byte*)heapAllocator.Allocate(Bytes, sizeof(byte), initMemory: false);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }
            heapAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void LocalAllocatorAlloc()
        {
            byte* buffer = localAllocator.Allocate<byte>(Bytes);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }
            localAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void LocalAllocatorAllocUnitializated()
        {
            byte* buffer = (byte*)localAllocator.Allocate(Bytes, sizeof(byte), initMemory: false);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }
            localAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void CAllocatorAlloc()
        {
            byte* buffer = cAllocator.Allocate<byte>(Bytes);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }
            cAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void CAllocatorAllocUnitializated()
        {
            byte* buffer = (byte*)cAllocator.Allocate(Bytes, sizeof(byte), initMemory: false);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }
            cAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void ArenaAllocatorAlloc()
        {
            byte* buffer = arenaAllocator.Allocate<byte>(Bytes);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }
            arenaAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void ArenaAllocatorAllocUnitializated()
        {
            byte* buffer = (byte*)arenaAllocator.Allocate(Bytes, sizeof(byte), initMemory: false);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }
            arenaAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void StackAllocatorAlloc()
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
        unsafe public void StackAllocatorAllocUnitializated()
        {
            byte* buffer = (byte*)stackAllocator.Allocate(Bytes, sizeof(byte), initMemory: false);
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
        unsafe public void PoolAllocatorAlloc()
        {
            byte* buffer = poolAllocator.Allocate<byte>(Bytes);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }
            poolAllocator.Free(buffer);
        }

        [Benchmark]
        unsafe public void PoolAllocatorAllocUnitializated()
        {
            byte* buffer = (byte*)poolAllocator.Allocate(Bytes, sizeof(byte), initMemory: false);
            for (int i = 0; i < Bytes; i++)
            {
                unchecked
                {
                    buffer[i] = (byte)i;
                }
            }
            poolAllocator.Free(buffer);
        }
    }
}
