using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NativeCollections.Utility
{
    unsafe public static class NativeCollectionUtilities
    {
        public static bool Contains<T>(void* pointer, int length, T value)
        {
            return Contains(pointer, 0, length, value, EqualityComparer<T>.Default);
        }

        public static bool Contains<T>(void* pointer, int lo, int hi, T value)
        {
            return Contains(pointer, lo, hi, value, EqualityComparer<T>.Default);
        }

        public static bool Contains<T>(void* pointer, int lo, int hi, T value, IEqualityComparer<T> comparer)
        {
            return IndexOf(pointer, lo, hi, value, comparer) >= 0;
        }

        public static int IndexOf<T>(void* pointer, int length, T value)
        {
            return IndexOf(pointer, 0, length, value, EqualityComparer<T>.Default);
        }

        public static int IndexOf<T>(void* pointer, int lo, int hi, T value)
        {
            return IndexOf(pointer, lo, hi, value, EqualityComparer<T>.Default);
        }

        public static int IndexOf<T>(void* pointer, int lo, int hi, T value, IEqualityComparer<T> comparer)
        {
            if (lo > hi)
                throw new ArgumentOutOfRangeException($"lo cannot be greater than hi: {lo} > {hi}");

            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            for(int i = lo; i < hi; i++)
            {
                ref T currentValue = ref Unsafe.Add(ref startAddress, i);
                if(comparer.Equals(currentValue, value))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int LastIndexOf<T>(void* pointer, int length, T value)
        {
            return LastIndexOf(pointer, 0, length, value, EqualityComparer<T>.Default);
        }

        public static int LastIndexOf<T>(void* pointer, int lo, int hi, T value)
        {
            return LastIndexOf(pointer, lo, hi, value, EqualityComparer<T>.Default);
        }

        public static int LastIndexOf<T>(void* pointer, int lo, int hi, T value, IEqualityComparer<T> comparer)
        {
            if (lo > hi)
                throw new ArgumentOutOfRangeException($"lo cannot be greater than hi: {lo} > {hi}");

            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            for (int i = hi - 1; i >= lo; i--)
            {
                ref T currentValue = ref Unsafe.Add(ref startAddress, i);
                if (comparer.Equals(currentValue, value))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int BinarySearch<T>(void* pointer, int length, T value)
        {
            return BinarySearch(pointer, 0, length, value, Comparer<T>.Default);
        }

        public static int BinarySearch<T>(void* pointer, int lo, int hi, T value)
        {
            return BinarySearch(pointer, lo, hi, value, Comparer<T>.Default);
        }

        public static int BinarySearch<T>(void* pointer, int lo, int hi, T value, IComparer<T> comparer)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            while(lo < hi)
            {
                int mid = lo + (hi - lo) / 2;
                ref T currentValue = ref Unsafe.Add(ref startAddress, mid);
                int comp = comparer.Compare(value, currentValue);

                if (comp == 0)
                {
                    return mid;
                }
                if(comp < 0)
                {
                    hi = mid - 1;
                }
                else
                {
                    lo = mid + 1;
                }
            }

            return ~lo;
        }

        public static int ReplaceAll<T>(void* pointer, int length, T value, T newValue)
        {
            return ReplaceAll(pointer, 0, length, value, newValue, EqualityComparer<T>.Default);
        }

        public static int ReplaceAll<T>(void* pointer, int lo, int hi, T value, T newValue)
        {
            return ReplaceAll(pointer, lo, hi, value, newValue, EqualityComparer<T>.Default);
        }

        public static int ReplaceAll<T>(void* pointer, int lo, int hi, T value, T newValue, IEqualityComparer<T> comparer)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            int count = 0;
            for (int i = lo; i < hi; i++)
            {
                ref T currentValue = ref Unsafe.Add(ref startAddress, i);
                if (comparer.Equals(currentValue, value))
                {
                    currentValue = newValue;
                    count++;
                }
            }

            return count;
        }

        public static void Reverse<T>(void* pointer, int length)
        {
            Reverse<T>(pointer, 0, length);
        }

        public static void Reverse<T>(void* pointer, int lo, int hi)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            while (lo < hi)
            {
                ref T a = ref Unsafe.Add(ref startAddress, lo);
                ref T b = ref Unsafe.Add(ref startAddress, hi);

                T temp = a;
                a = b;
                b = temp;

                lo++;
                hi--;
            }
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}
