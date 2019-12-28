using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NativeCollections.Utility
{
    unsafe public static class NativeCollectionUtility
    {
        public static int ReplaceAll<T>(void* pointer, int length, T value, T newValue)
        {
            return ReplaceAll(pointer, 0, length - 1, value, newValue, EqualityComparer<T>.Default);
        }

        public static int ReplaceAll<T>(void* pointer, int start, int end, T value, T newValue)
        {
            return ReplaceAll(pointer, start, end, value, newValue, EqualityComparer<T>.Default);
        }

        public static int ReplaceAll<T>(void* pointer, int start, int end, T value, T newValue, IEqualityComparer<T> comparer)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            int count = 0;
            for(int i = start; i < end; i++)
            {
                ref T currentValue = ref Unsafe.Add(ref startAddress, i);
                if(comparer.Equals(currentValue, value))
                {
                    currentValue = newValue;
                    count++;
                }
            }

            return count;
        }

        public static void Reverse<T>(void* pointer, int length)
        {
            Reverse<T>(pointer, 0, length - 1);
        }

        public static void Reverse<T>(void* pointer, int start, int end)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            while (start < end)
            {
                ref T a = ref Unsafe.Add(ref startAddress, start);
                ref T b = ref Unsafe.Add(ref startAddress, end);

                Swap(ref a, ref b);
                start++;
                end--;
            }
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static int IndexOf<T>(void* pointer, int length, T value)
        {
            return IndexOf(pointer, 0, length - 1, value, EqualityComparer<T>.Default);
        }

        public static int IndexOf<T>(void* pointer, int start, int end, T value)
        {
            return IndexOf(pointer, start, end, value, EqualityComparer<T>.Default);
        }

        public static int IndexOf<T>(void* pointer, int start, int end, T value, IEqualityComparer<T> comparer)
        {
            if (start > end)
                throw new ArgumentOutOfRangeException($"start cannot be greater than end: {start} > {end}");

            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            for(int i = start; i < end; i++)
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
            return LastIndexOf(pointer, 0, length - 1, value, EqualityComparer<T>.Default);
        }

        public static int LastIndexOf<T>(void* pointer, int start, int end, T value)
        {
            return LastIndexOf(pointer, start, end, value, EqualityComparer<T>.Default);
        }

        public static int LastIndexOf<T>(void* pointer, int start, int end, T value, IEqualityComparer<T> comparer)
        {
            if (start > end)
                throw new ArgumentOutOfRangeException($"start cannot be greater than end: {start} > {end}");

            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            for (int i = end - 1; i >= start; i--)
            {
                ref T currentValue = ref Unsafe.Add(ref startAddress, i);
                if (comparer.Equals(currentValue, value))
                {
                    return i;
                }
            }

            return -1;
        }
    
        public static int BinarySearch<T>(void* pointer, int length, T value) where T: IComparable<T>
        {
            return BinarySearch(pointer, 0, length - 1, value);
        }

        public static int BinarySearch<T>(void* pointer, int start, int end, T value) where T: IComparable<T>
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            while(start < end)
            {
                int mid = start + (end - start) / 2;

                ref T currentValue = ref Unsafe.Add(ref startAddress, mid);
                if (value.CompareTo(currentValue) == 0)
                    return mid;

                if(value.CompareTo(currentValue) < 0)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid + 1;
                }
            }

            return -1;
        }
    
        public static void QuickSort<T>(void* pointer, int start, int end) where T: IComparable<T>
        {
            if(start < end)
            {
                int p = QuickSortPartition<T>(pointer, start, end);
                QuickSort<T>(pointer, start, p - 1);
                QuickSort<T>(pointer, p + 1, end);
            }
        }

        private static int QuickSortPartition<T>(void* pointer, int start, int end) where T : IComparable<T>
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);
            ref T value = ref Unsafe.Add(ref startAddress, end);

            int i = start - 1;
            int length = end - 1;

            for (int j = start; j <= length; j++)
            {
                ref T leftValue = ref Unsafe.Add(ref startAddress, j);
              
                if(leftValue.CompareTo(value) <= 0)
                {
                    i++;
                    Swap(ref Unsafe.Add(ref startAddress, i), ref leftValue);
                }
            }

            Swap(ref value, ref Unsafe.Add(ref startAddress, i + 1));
            return i + 1;
        }
    }
}
