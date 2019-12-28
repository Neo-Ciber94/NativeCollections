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
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void Sort<T>(this NativeArray<T> array, int start) where T : unmanaged
        {
            Sort(array, start, array.Length - 1, Comparer<T>.Default);
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
            NativeCollectionUtilities.QuickSort<T>(pointer, start, end, comparer);
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
        /// <param name="comparison">Used for compare the elements to sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void Sort<T>(this NativeArray<T> array, int start, Comparison<T> comparison) where T : unmanaged
        {
            Sort(array, start, array.Length - 1, comparison);
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
            NativeCollectionUtilities.QuickSort<T>(pointer, start, end, comparison);
        }

        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void Sort<T, TSelect>(this NativeArray<T> array, Func<T, TSelect> selector) where T: unmanaged
        {
            Sort(array, 0, array.Length - 1, selector);
        }

        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void Sort<T, TSelect>(this NativeArray<T> array, int start, Func<T, TSelect> selector) where T : unmanaged
        {
            Sort(array, start, array.Length - 1, selector);
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
        public static void Sort<T, TSelect>(this NativeArray<T> array, int start, int end, Func<T, TSelect> selector) where T : unmanaged
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
            NativeCollectionUtilities.QuickSort(pointer, start, end, selector);
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
        /// <param name="value">The value to find.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static int BinarySearch<T>(this NativeArray<T> array, int start, T value) where T : unmanaged
        {
            return BinarySearch(array, start, array.Length - 1, value, Comparer<T>.Default);
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
        /// Performs a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="comparison">Used for compared the elements during the search.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static int BinarySearch<T>(this NativeArray<T> array, T value, Comparison<T> comparison) where T : unmanaged
        {
            return BinarySearch(array, 0, array.Length - 1, value, comparison);
        }

        /// <summary>
        /// Performs a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="start">The start index.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="comparison">Used for compared the elements during the search.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static int BinarySearch<T>(this NativeArray<T> array, int start, T value, Comparison<T> comparison) where T : unmanaged
        {
            return BinarySearch(array, start, array.Length - 1, value, comparison);
        }

        /// <summary>
        /// Performs a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="comparison">Used for compared the elements during the search.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static int BinarySearch<T>(this NativeArray<T> array, int start, int end, T value, Comparison<T> comparison) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            if(start > end || start < 0 || end < 0 || start > array.Length || end > array.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            return NativeCollectionUtilities.BinarySearch(array._buffer, start, end, value, comparison);
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

        public static void ForEach<T>(this NativeArray<T> array, Action<T> action) where T: unmanaged
        {
            foreach(T value in array)
            {
                action(value);
            }
        }

        public static void ForEachRef<T>(this NativeArray<T> array, RefAction<T> action) where T : unmanaged
        {
            foreach (ref T value in array)
            {
                action(ref value);
            }
        }

        public static void ForEachIndex<T>(this NativeArray<T> array, RefIndexedAction<T> action) where T : unmanaged
        {
            int i = 0;
            foreach (ref T value in array)
            {
                action(ref value, i++);
            }
        }
    }

    public delegate void RefAction<T>(ref T value);

    public delegate void RefIndexedAction<T>(ref T value, int index);
}
