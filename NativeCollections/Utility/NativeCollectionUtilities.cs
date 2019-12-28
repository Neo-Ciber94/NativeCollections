using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NativeCollections.Utility
{
    unsafe public static class NativeCollectionUtilities
    {
        public static bool Contains<T>(void* pointer, int length, T value)
        {
            return Contains(pointer, 0, length - 1, value, EqualityComparer<T>.Default);
        }

        public static bool Contains<T>(void* pointer, int start, int end, T value)
        {
            return Contains(pointer, start, end, value, EqualityComparer<T>.Default);
        }

        public static bool Contains<T>(void* pointer, int start, int end, T value, IEqualityComparer<T> comparer)
        {
            return IndexOf(pointer, start, end, value, comparer) >= 0;
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

        public static int BinarySearch<T>(void* pointer, int length, T value)
        {
            return BinarySearch(pointer, 0, length - 1, value, Comparer<T>.Default);
        }

        public static int BinarySearch<T>(void* pointer, int start, int end, T value)
        {
            return BinarySearch(pointer, start, end, value, Comparer<T>.Default);
        }

        public static int BinarySearch<T>(void* pointer, int start, int end, T value, IComparer<T> comparer)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            while(start < end)
            {
                int mid = start + (end - start) / 2;
                ref T currentValue = ref Unsafe.Add(ref startAddress, mid);
                int comp = comparer.Compare(value, currentValue);

                if (comp == 0)
                {
                    return mid;
                }
                if(comp < 0)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid + 1;
                }
            }

            return ~start;
        }

        public static int BinarySearch<T>(void* pointer, int length, T value, Comparison<T> comparison)
        {
            return BinarySearch(pointer, 0, length - 1, value, comparison);
        }

        public static int BinarySearch<T>(void* pointer, int start, int end, T value, Comparison<T> comparison)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            while (start < end)
            {
                int mid = start + (end - start) / 2;
                ref T currentValue = ref Unsafe.Add(ref startAddress, mid);
                int comp = comparison(value, currentValue);

                if (comp == 0)
                {
                    return mid;
                }
                if (comp < 0)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid + 1;
                }
            }

            return ~start;
        }

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
            for (int i = start; i < end; i++)
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
            Reverse<T>(pointer, 0, length - 1);
        }

        public static void Reverse<T>(void* pointer, int start, int end)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            while (start < end)
            {
                ref T a = ref Unsafe.Add(ref startAddress, start);
                ref T b = ref Unsafe.Add(ref startAddress, end);

                T temp = a;
                a = b;
                b = temp;

                start++;
                end--;
            }
        }

        public static void QuickSort<T>(void* pointer, int length)
        {
            QuickSort(pointer, 0, length - 1, Comparer<T>.Default);
        }

        public static void QuickSort<T>(void* pointer, int start, int end)
        {
            QuickSort(pointer, start, end, Comparer<T>.Default);
        }

        public static void QuickSort<T>(void* pointer, int start, int end, IComparer<T> comparer)
        {
            if (start < end)
            {
                int p = QuickSortPartition(pointer, start, end, comparer);
                QuickSort(pointer, start, p - 1, comparer);
                QuickSort(pointer, p + 1, end, comparer);
            }
        }

        public static void QuickSort<T>(void* pointer, int length, Comparison<T> comparison)
        {
            QuickSort(pointer, 0, length, comparison);
        }

        public static void QuickSort<T>(void* pointer, int start, int end, Comparison<T> comparison)
        {
            if (start < end)
            {
                int p = QuickSortPartition(pointer, start, end, comparison);
                QuickSort(pointer, start, p - 1, comparison);
                QuickSort(pointer, p + 1, end, comparison);
            }
        }

        public static void QuickSort<T, TSelect>(void* pointer, int length, Func<T, TSelect> selector)
        {
            QuickSort(pointer, 0, length - 1, selector, Comparer<TSelect>.Default);
        }

        public static void QuickSort<T, TSelect>(void* pointer, int start, int end, Func<T, TSelect> selector)
        {
            QuickSort(pointer, start, end, selector, Comparer<TSelect>.Default);
        }

        public static void QuickSort<T, TSelect>(void* pointer, int start, int end, Func<T, TSelect> selector, IComparer<TSelect> comparer)
        {
            if (start < end)
            {
                int p = QuickSortPartition(pointer, start, end, selector, comparer);
                QuickSort(pointer, start, p - 1, selector, comparer);
                QuickSort(pointer, p + 1, end, selector, comparer);
            }
        }

        private static int QuickSortPartition<T>(void* pointer, int start, int end, Comparison<T> compariso)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);
            ref T value = ref Unsafe.Add(ref startAddress, end);

            int i = start - 1;
            int length = end - 1;

            for (int j = start; j <= length; j++)
            {
                ref T leftValue = ref Unsafe.Add(ref startAddress, j);

                if (compariso(leftValue, value) <= 0)
                {
                    i++;
                    Swap(ref Unsafe.Add(ref startAddress, i), ref leftValue);
                }
            }

            Swap(ref value, ref Unsafe.Add(ref startAddress, i + 1));
            return i + 1;
        }

        private static int QuickSortPartition<T>(void* pointer, int start, int end, IComparer<T> comparer)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);
            ref T value = ref Unsafe.Add(ref startAddress, end);

            int i = start - 1;
            int length = end - 1;

            for (int j = start; j <= length; j++)
            {
                ref T leftValue = ref Unsafe.Add(ref startAddress, j);

                if (comparer.Compare(leftValue, value) <= 0)
                {
                    i++;
                    Swap(ref Unsafe.Add(ref startAddress, i), ref leftValue);
                }
            }

            Swap(ref value, ref Unsafe.Add(ref startAddress, i + 1));
            return i + 1;
        }

        private static int QuickSortPartition<T, TSelect>(void* pointer, int start, int end, Func<T, TSelect> selector, IComparer<TSelect> comparer)
        {
            ref T startAddress = ref Unsafe.AsRef<T>(pointer);
            ref T value = ref Unsafe.Add(ref startAddress, end);

            int i = start - 1;
            int length = end - 1;

            for (int j = start; j <= length; j++)
            {
                ref T leftValue = ref Unsafe.Add(ref startAddress, j);
                TSelect a = selector(leftValue);
                TSelect b = selector(value);

                if (comparer.Compare(a, b) <= 0)
                {
                    i++;
                    Swap(ref Unsafe.Add(ref startAddress, i), ref leftValue);
                }
            }

            Swap(ref value, ref Unsafe.Add(ref startAddress, i + 1));
            return i + 1;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}
