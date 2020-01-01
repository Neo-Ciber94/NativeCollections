using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace NativeCollections.Utility
{
    unsafe public static class NativeSortUtilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(void* pointer, int length) where T: IComparable<T>
        {
            Sort(pointer, 0, length - 1, false, Comparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(void* pointer, int length, Comparison<T> comparison)
        {
            Sort(pointer, 0, length - 1, false, comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(void* pointer, int length, IComparer<T> comparer)
        {
            Sort(pointer, 0, length - 1, false, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(void* pointer, int low, int high, bool decending, Comparison<T> comparison)
        {
            QuickSort(pointer, low, high, decending, comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(void* pointer, int low, int high, bool decending, IComparer<T> comparer)
        {
            QuickSort(pointer, low, high, decending, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortBy<T, TSelect>(void* pointer, int low, int high, bool decending, IComparer<TSelect> comparer, Func<T, TSelect> selector)
        {
            QuickSort(pointer, low, high, decending, comparer, selector);
        }

        /// <summary>
        /// Determines whether the elements in the specified pointer are sorted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="low">The start.<param>
        /// <param name="high">The end.</param>
        /// <param name="decending">if <c>true</c> will check if are sorted by decending; otherwise by ascending.</param>
        /// <returns>
        ///   <c>true</c> if the elements in the specified pointer are sorted; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSorted<T>(void* pointer, int low, int high, bool decending) where T: IComparable<T>
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(low < high, "Invalid range");

            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            int next = low;
            while (++next < high)
            {
                ref T x = ref Unsafe.Add(ref ptr, low);
                ref T y = ref Unsafe.Add(ref ptr, next);

                int comp = x.CompareTo(y);

                if (decending)
                {
                    if (comp < 0)
                        return false;
                }
                else
                {
                    if (comp > 0)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the elements in the specified pointer are sorted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="low">The start.<param>
        /// <param name="high">The end.</param>
        /// <param name="decending">if <c>true</c> will check if are sorted by decending; otherwise by ascending.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        ///   <c>true</c> if the elements in the specified pointer are sorted; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSorted<T>(void* pointer, int low, int high, bool decending, IComparer<T> comparer)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(low < high, "Invalid range");

            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            int next = low;
            while (++next < high)
            {
                ref T x = ref Unsafe.Add(ref ptr, low);
                ref T y = ref Unsafe.Add(ref ptr, next);

                int comp = comparer.Compare(x, y);
                if (decending)
                {
                    if (comp < 0)
                        return false;
                }
                else
                {
                    if (comp > 0)
                        return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Determines whether the elements in the specified pointer are sorted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="low">The start.<param>
        /// <param name="high">The end.</param>
        /// <param name="decending">if <c>true</c> will check if are sorted by decending; otherwise by ascending.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns>
        ///   <c>true</c> if the elements in the specified pointer are sorted; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSorted<T>(void* pointer, int low, int high, bool decending, Comparison<T> comparison)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(low < high, "Invalid range");

            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            int next = low;
            while(++next < high)
            {
                ref T x = ref Unsafe.Add(ref ptr, low);
                ref T y = ref Unsafe.Add(ref ptr, next);

                int comp = comparison(x, y);
                if (decending)
                {
                    if (comp < 0)
                        return false;
                }
                else
                {
                    if (comp > 0)
                        return false;
                }
            }

            return true;
        }

        #region InsertionSort
        public static void InsertionSort<T>(void* pointer, int low, int high, bool decending, IComparer<T> comparer)
        {
            InsertionSort<T>(pointer, low, high, decending, comparer.Compare);
        }

        public static void InsertionSort<T>(void* pointer, int low, int high, bool decending, Comparison<T> comparison)
        {
            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            for (int i = low; i <= high; i++)
            {
                ref T x = ref Unsafe.Add(ref ptr, i);
                int min = i;

                for (int j = i + 1; j <= high; j++)
                {
                    ref T value1 = ref Unsafe.Add(ref ptr, j);
                    ref T value2 = ref Unsafe.Add(ref ptr, min);

                    int comp = decending ? comparison(value2, value1) : comparison(value1, value2);
                    if (comp < 0)
                    {
                        min = j;
                    }
                }

                if(min != i)
                {
                    Swap(ref x, ref Unsafe.Add(ref ptr, min));
                }
            }
        }

        public static void InsertionSort<T, TSelect>(void* pointer, int low, int high, bool decending, IComparer<TSelect> comparer, Func<T, TSelect> selector)
        {
            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            for (int i = low; i <= high; i++)
            {
                ref T x = ref Unsafe.Add(ref ptr, i);
                int min = i;

                for (int j = i + 1; j <= high; j++)
                {
                    ref T value1 = ref Unsafe.Add(ref ptr, j);
                    ref T value2 = ref Unsafe.Add(ref ptr, min);

                    TSelect a = selector(value1);
                    TSelect b = selector(value2);

                    int comp = decending ? comparer.Compare(b, a) : comparer.Compare(a, b);
                    if (comp < 0)
                    {
                        min = j;
                    }
                }

                if (min != i)
                {
                    Swap(ref x, ref Unsafe.Add(ref ptr, min));
                }
            }
        }
        #endregion

        #region QuickSort
        public static void QuickSort<T>(void* pointer, int low, int high, bool decending, IComparer<T> comparer)
        {
            QuickSort<T>(pointer, low, high, decending, comparer.Compare);
        }

        public static void QuickSort<T>(void* pointer, int low, int high, bool decending, Comparison<T> comparison)
        {
            if (low < high)
            {
                int p = QuickSortPartition(pointer, low, high, decending, comparison);
                QuickSort(pointer, low, p - 1, decending, comparison);
                QuickSort(pointer, p + 1, high, decending, comparison);
            }
        }

        private static int QuickSortPartition<T>(void* pointer, int low, int high, bool decending, Comparison<T> comparison)
        {
            ref T ptr = ref Unsafe.AsRef<T>(pointer);
            ref T pivot = ref Unsafe.Add(ref ptr, high);

            int i = low - 1;
            int length = high - 1;

            for (int j = low; j <= length; j++)
            {
                ref T value = ref Unsafe.Add(ref ptr, j);
                int comp = decending ? comparison(pivot, value) : comparison(value, pivot);
                if (comp <= 0)
                {
                    i++;
                    Swap(ref Unsafe.Add(ref ptr, i), ref value);
                }
            }

            Swap(ref Unsafe.Add(ref ptr, high), ref Unsafe.Add(ref ptr, i + 1));
            return i + 1;
        }

        public static void QuickSort<T, TSelect>(void* pointer, int low, int high, bool decending, IComparer<TSelect> comparer, Func<T, TSelect> selector)
        {
            if (low < high)
            {
                int p = QuickSortPartition(pointer, low, high, decending, comparer, selector);
                QuickSort(pointer, low, p - 1, decending, comparer, selector);
                QuickSort(pointer, p + 1, high, decending, comparer, selector);
            }
        }

        private static int QuickSortPartition<T, TSelect>(void* pointer, int low, int high, bool decending, IComparer<TSelect> comparer, Func<T, TSelect> selector)
        {
            ref T ptr = ref Unsafe.AsRef<T>(pointer);
            ref T pivot = ref Unsafe.Add(ref ptr, high);

            int i = low - 1;
            int length = high - 1;

            for (int j = low; j <= length; j++)
            {
                ref T value = ref Unsafe.Add(ref ptr, j);
                TSelect a = selector(value);
                TSelect b = selector(pivot);

                int comp = decending ? comparer.Compare(b, a) : comparer.Compare(a, b);
                if (comp < 0)
                {
                    i++;
                    Swap(ref Unsafe.Add(ref ptr, i), ref value);
                }
            }

            Swap(ref Unsafe.Add(ref ptr, high), ref Unsafe.Add(ref ptr, i + 1));
            return i + 1;
        }
        #endregion

        private static void Swap<T>(ref T x, ref T y)
        {
            T temp = x;
            x = y;
            y = temp;
        }
    }
}
