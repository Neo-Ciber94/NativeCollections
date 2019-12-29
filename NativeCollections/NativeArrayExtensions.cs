using System;
using System.Collections.Generic;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Extensions for <see cref="NativeArray{T}"/>.
    /// </summary>
    unsafe public static class NativeArrayExtensions
    {
        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void Sort<T>(this NativeArray<T> array) where T : unmanaged
        {
            Sort(array, 0, array.Length - 1, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void Sort<T>(this NativeArray<T> array, int start, int end) where T : unmanaged
        {
            Sort(array, start, end, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void Sort<T>(this NativeArray<T> array, int start, int end, IComparer<T> comparer) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > array.Length || end > array.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            void* pointer = array._buffer;
            NativeSortUtilities.Sort(pointer, start, end, false, comparer);
        }

        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="comparison">Used for compare the elements to sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void Sort<T>(this NativeArray<T> array, Comparison<T> comparison) where T : unmanaged
        {
            Sort(array, 0, array.Length - 1, comparison);
        }

        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="comparison">Used for compare the elements to sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void Sort<T>(this NativeArray<T> array, int start, int end, Comparison<T> comparison) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > array.Length || end > array.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            void* pointer = array._buffer;
            NativeSortUtilities.Sort(pointer, start, end, false, comparison);
        }

        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void SortBy<T, TSelect>(this NativeArray<T> array, Func<T, TSelect> selector) where T: unmanaged
        {
            SortBy(array, 0, array.Length - 1, Comparer<TSelect>.Default, selector);
        }

        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void SortBy<T, TSelect>(this NativeArray<T> array, int start, int end, Func<T, TSelect> selector) where T : unmanaged
        {
            SortBy(array, start, end, Comparer<TSelect>.Default, selector);
        }

        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="comparer">The comparer used for sorting</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void SortBy<T, TSelect>(this NativeArray<T> array, int start, int end, IComparer<TSelect> comparer, Func<T, TSelect> selector)where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > array.Length || end > array.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            void* pointer = array._buffer;
            NativeSortUtilities.SortBy(pointer, start, end, false, comparer, selector);
        }

        /// <summary>
        /// Sorts by decending the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void SortByDecending<T, TSelect>(this NativeArray<T> array, Func<T, TSelect> selector) where T : unmanaged
        {
            SortByDecending(array, 0, array.Length - 1, Comparer<TSelect>.Default, selector);
        }

        /// <summary>
        /// Sorts by decending the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void SortByDecending<T, TSelect>(this NativeArray<T> array, int start, int end, Func<T, TSelect> selector) where T : unmanaged
        {
            SortByDecending(array, start, end, Comparer<TSelect>.Default, selector);
        }

        /// <summary>
        /// Sorts by decending the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="comparer">The comparer used for sorting</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void SortByDecending<T, TSelect>(this NativeArray<T> array, int start, int end, IComparer<TSelect> comparer, Func<T, TSelect> selector) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > array.Length || end > array.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            void* pointer = array._buffer;
            NativeSortUtilities.SortBy(pointer, start, end, true, comparer, selector);
        }

        /// <summary>
        /// Performs a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static int BinarySearch<T>(this NativeArray<T> array, T value) where T : unmanaged
        {
            return BinarySearch(array, 0, array.Length - 1, value, Comparer<T>.Default);
        }

        /// <summary>
        /// Performs a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static int BinarySearch<T>(this NativeArray<T> array, int start, int end, T value) where T : unmanaged
        {
            return BinarySearch(array, start, end, value, Comparer<T>.Default);
        }

        /// <summary>
        /// Performs a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="comparer">The comparer used to find the elements.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static int BinarySearch<T>(this NativeArray<T> array, int start, int end, T value, IComparer<T> comparer) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > array.Length || end > array.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            return NativeCollectionUtilities.BinarySearch(array._buffer, start, end, value, comparer);
        }

        /// <summary>
        /// Determines whether exists a value that match the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="match">The predicate used.</param>
        /// <returns><c>true</c> if a value exists; otherwise <c>false</c>.</returns>
        public static bool Exists<T>(this NativeArray<T> array, Predicate<T> match) where T: unmanaged
        {
            for(int i = 0; i < array.Length; i++)
            {
                if (match(array[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Performs a foreach loop over the elements of the given array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="action">The action to perform over each element.</param>
        public static void ForEach<T>(this NativeArray<T> array, Action<T> action) where T: unmanaged
        {
            int length = array.Length;
            for(int i = 0; i < length; i++)
            {
                action(array[i]);
            }
        }

        /// <summary>
        /// Performs a foreach loop over the index-element values of the given array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="action">The action to perform over each element and its index.</param>
        public static void ForEach<T>(this NativeArray<T> array, Action<T, int> action) where T : unmanaged
        {
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                action(array[i], i);
            }
        }

        /// <summary>
        /// Performs a ref-like foreach loop over the elements of the array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="action">The action to perform over each reference of the elements.</param>
        public static void ForEachRef<T>(this NativeArray<T> array, RefAction<T> action) where T : unmanaged
        {
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                action(ref array[i]);
            }
        }

        /// <summary>
        /// Performs a ref-like foreach loop over the index-element values of the array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="action">The action to perform over each reference of the elements and its index.</param>
        public static void ForEachRef<T>(this NativeArray<T> array, RefIndexedAction<T> action) where T : unmanaged
        {
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                action(ref array[i], i);
            }
        }

        /// <summary>
        /// Releases the resources used for this array and each of its elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="disposing">if set to <c>true</c> will call dispose for each element in the array.</param>
        public static void Dispose<T>(this ref NativeArray<T> array, bool disposing) where T : unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (ref var e in array)
                    {
                        e.Dispose();
                    }
                }
            }
            finally
            {
                array.Dispose();
            }
        }
    }
}
