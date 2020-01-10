using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public static void Sort<T>(this NativeArray<T> array) where T : unmanaged, IComparable<T>
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
        public static void Sort<T>(this NativeArray<T> array, int start, int end) where T : unmanaged, IComparable<T>
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
        public static void SortBy<T, TSelect>(this NativeArray<T> array, Func<T, TSelect> selector) where T: unmanaged where TSelect: IComparable<TSelect>
        {
            SortBy(array, 0, array.Length - 1, Comparer<TSelect>.Default, selector);
        }

        /// <summary>
        /// Sorts the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="comparer">The comparer used for sorting</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void SortBy<T, TSelect>(this NativeArray<T> array, IComparer<TSelect> comparer, Func<T, TSelect> selector) where T : unmanaged
        {
            SortBy(array, 0, array.Length - 1, comparer, selector);
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
        public static void SortBy<T, TSelect>(this NativeArray<T> array, int start, int end, Func<T, TSelect> selector) where T : unmanaged where TSelect: IComparable<TSelect>
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
        public static void SortBy<T, TSelect>(this NativeArray<T> array, int start, int end, IComparer<TSelect> comparer, Func<T, TSelect> selector) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > array.Length || end > array.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            NativeSortUtilities.SortBy(array._buffer, start, end, false, comparer, selector);
        }

        /// <summary>
        /// Sorts by decending the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void SortByDecending<T, TSelect>(this NativeArray<T> array, Func<T, TSelect> selector) where T : unmanaged where TSelect : IComparable<TSelect>
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
        public static void SortByDecending<T, TSelect>(this NativeArray<T> array, int start, int end, Func<T, TSelect> selector) where T : unmanaged where TSelect : IComparable<TSelect>
        {
            SortByDecending(array, start, end, Comparer<TSelect>.Default, selector);
        }

        /// <summary>
        /// Sorts by decending the content of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="array">The array to sort.</param>
        /// <param name="comparer">The comparer used for sorting</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static void SortByDecending<T, TSelect>(this NativeArray<T> array, IComparer<TSelect> comparer, Func<T, TSelect> selector) where T : unmanaged
        {
            SortByDecending(array, 0, array.Length - 1, comparer, selector);
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

            NativeSortUtilities.SortBy(array._buffer, start, end, true, comparer, selector);
        }

        /// <summary>
        /// Performs a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the array is invalid</exception>
        public static int BinarySearch<T>(this NativeArray<T> array, T value) where T : unmanaged, IComparable<T>
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
        public static int BinarySearch<T>(this NativeArray<T> array, int start, int end, T value) where T : unmanaged, IComparable<T>
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

            return UnsafeUtilities.BinarySearch(array._buffer, start, end, value, comparer);
        }

        /// <summary>
        /// Finds the first element that matchs the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>The first element that matchs the condition or null if not found.</returns>
        public static T? FindFirst<T>(this NativeArray<T> array, Predicate<T> predicate) where T : unmanaged
        {
            foreach (ref var e in array)
            {
                if (predicate(e))
                {
                    return e;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the last element that matchs the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>The last element that matchs the condition or null if not found.</returns>
        public static T? FindLast<T>(this NativeArray<T> array, Predicate<T> predicate) where T : unmanaged
        {
            for (int i = array.Length - 1; i >= 0; --i)
            {
                if (predicate(array[i]))
                {
                    return array[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the first element that matchs the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>A reference to the first element that matchs the condition.</returns>
        /// <exception cref="InvalidOperationException">If no value is not found.</exception>
        public static ref T FindFirstRef<T>(this NativeArray<T> array, Predicate<T> predicate) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            foreach (ref var e in array)
            {
                if (predicate(e))
                {
                    return ref e;
                }
            }

            throw new InvalidOperationException("No value found");
        }

        /// <summary>
        /// Finds the last element that matchs the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>A reference to the last element that matchs the condition.</returns>
        /// <exception cref="InvalidOperationException">If no value is not found.</exception>
        public static ref T FindLastRef<T>(this NativeArray<T> array, Predicate<T> predicate) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            for (int i = array.Length - 1; i >= 0; --i)
            {
                if (predicate(array[i]))
                {
                    return ref array[i];
                }
            }

            throw new InvalidOperationException("No value found");
        }

        /// <summary>
        /// Finds alls elements that match the specified predicate.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>An NativeArray with all the matches.</returns>
        public static NativeArray<T> FindAll<T>(this NativeArray<T> array, Predicate<T> predicate) where T : unmanaged
        {
            if (!array.IsValid)
            {
                return default;
            }

            NativeList<T> matches = new NativeList<T>(4, array.GetAllocator()!);

            foreach (ref var e in array)
            {
                if (predicate(e))
                {
                    matches.Add(e);
                }
            }

            return matches.ToNativeArrayAndDispose();
        }

        /// <summary>
        /// Finds the index of the first element that meet the given condition.
        /// </summary>
        /// <typeparam name="T">Type of the element.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>The index of the first element that meet the condition.</returns>
        public static int FindIndex<T>(this NativeArray<T> array, Predicate<T> predicate) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            for (int i = 0; i < array.Length; ++i)
            {
                if (predicate(array[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the index of the last element that meet the given condition.
        /// </summary>
        /// <typeparam name="T">Type of the element.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>The index of the last element that meet the condition.</returns>
        public static int FindLastIndex<T>(this NativeArray<T> array, Predicate<T> predicate) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            for (int i = array.Length - 1; i >= 0; --i)
            {
                if (predicate(array[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines whether exists a value that match the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The predicate used.</param>
        /// <returns><c>true</c> if a value exists; otherwise <c>false</c>.</returns>
        public static bool AnyMatch<T>(this NativeArray<T> array, Predicate<T> predicate) where T: unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            for (int i = 0; i < array.Length; ++i)
            {
                if (predicate(array[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether all the elements in the array meet the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The condition to use.</param>
        /// <returns>
        ///     <c>true</c> if all the elements meet the condition; otherwise <c>false</c>.
        /// </returns>
        public static bool AllMatch<T>(this NativeArray<T> array, Predicate<T> predicate) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            for (int i = 0; i < array.Length; ++i)
            {
                if (!predicate(array[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether neither of the elements in the array meet the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="predicate">The condition to use.</param>
        /// <returns>
        ///     <c>true</c> if none the elements meet the condition; otherwise <c>false</c>.
        /// </returns>
        public static bool NoneMatch<T>(this NativeArray<T> array, Predicate<T> predicate) where T : unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            for (int i = 0; i < array.Length; ++i)
            {
                if (predicate(array[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Performs a foreach loop over the elements of the given array.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="action">The action to perform over each element.</param>
        public static void ForEach<T>(this NativeArray<T> array, Action<T> action) where T: unmanaged
        {
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            int length = array.Length;
            for(int i = 0; i < length; ++i)
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
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            int length = array.Length;
            for (int i = 0; i < length; ++i)
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
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            int length = array.Length;
            for (int i = 0; i < length; ++i)
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
            if (!array.IsValid)
            {
                throw new ArgumentException("NativeArray is invalid");
            }

            int length = array.Length;
            for (int i = 0; i < length; ++i)
            {
                action(ref array[i], i);
            }
        }
    }
}
