using System;
using System.Runtime.CompilerServices;

namespace NativeCollections
{
    public static class NativeArray
    {
        unsafe public static void* GetUnsafePointer<T>(ref NativeArray<T> array) where T: unmanaged
        {
            if (!array.IsValid)
                throw new ArgumentException("The array is invalid");

            return array._buffer;
        }

        unsafe public static ref T GetReference<T>(ref NativeArray<T> array) where T: unmanaged
        {
            if (!array.IsValid)
                throw new ArgumentException("The array is invalid");

            return ref Unsafe.AsRef<T>(array._buffer);
        }

        public static NativeArray<int> Range(int start, int end)
        {
            if (start == end)
                return default;

            if(start > end)
            {
                int length = start - end;
                NativeArray<int> array = new NativeArray<int>(length);

                for(int i = 0; start > end; start--, i++)
                {
                    array[i] = start;
                }

                return array;
            }
            else
            {
                int length = end - start;
                NativeArray<int> array = new NativeArray<int>(length);

                for (int i = 0; start < end; start++, i++)
                {
                    array[i] = start;
                }

                return array;
            }
        }

        public static NativeArray<int> Range(int end)
        {
            if (end < 0)
                throw new ArgumentOutOfRangeException("end", $"0 > {end}");

            return Range(0, end);
        }

        public static NativeArray<int> RangeClosed(int start, int end)
        {
            if (start == end)
                return default;

            if(start > end)
            {
                return Range(start, end - 1);
            }

            return Range(start, end + 1);
        }

        public static NativeArray<int> RangeClosed(int end)
        {
            if (end == 0)
                return default;

            if (end < 0)
                throw new ArgumentOutOfRangeException("end", $"0 > {end}");

            return Range(0, end + 1);
        }
    }
}
