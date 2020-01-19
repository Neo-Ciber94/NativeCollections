using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace NativeCollectionsBenchmark
{
    [ShortRunJob]
    [MinColumn, MaxColumn]
    unsafe public class MemoryInitBenchmark
    {
        private byte* _memory;

        [Params(10, 16, 100, 128, 1000, 1024)]
        public int Size;

        [GlobalSetup]
        public void Setup()
        {
            _memory = (byte*)Marshal.AllocHGlobal(Size);
        }

        [GlobalCleanup]
        public void Clear()
        {
            Marshal.FreeHGlobal((IntPtr)_memory);
        }

        [Benchmark]
        public void UnsafeInitBlock()
        {
            Unsafe.InitBlock(_memory, 0, (uint)Size);
        }

        [Benchmark]
        public void UnsafeInitBlockUnaligned()
        {
            Unsafe.InitBlockUnaligned(_memory, 0, (uint)Size);
        }

        [Benchmark]
        public void ZeroMemory()
        {
            ZeroMemory(_memory, (uint)Size);
        }

        [Benchmark]
        public void InitMemory4()
        {
            int length = Size;
            int pos = 0;

            if (length >= 4)
            {
                while (pos < length)
                {
                    *(int*)(_memory + pos) = 0;
                    pos += 4;
                    length -= 4;
                }
            }

            for (; length > 0; length--)
            {
                _memory[pos++] = 0;
            }
        }

        [Benchmark]
        public void InitMemory8()
        {
            int length = Size;
            int pos = 0;

            if(length >= 8)
            {
                while(pos < length)
                {
                    *(long*)(_memory + pos) = 0;

                    pos += 8;
                    length -= 8;
                }
            }

            if(length >= 4)
            {
                while (pos < length)
                {
                    *(int*)(_memory + pos) = 0;

                    pos += 4;
                    length -= 4;
                }
            }

            for(; length > 0; length--)
            {
                _memory[pos++] = 0;
            }
        }

        [DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
        static extern void ZeroMemory(void* dest, uint size);
    }
}
