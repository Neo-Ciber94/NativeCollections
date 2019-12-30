using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public static class NativeListExtensions
    {
        /// <summary>
        /// Releases the resources used for this list and each of its elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="disposing">if set to <c>true</c> will call dispose for each element in the list.</param>
        public static void Dispose<T>(this ref NativeList<T> list, bool disposing) where T: unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (ref var e in list)
                    {
                        e.Dispose();
                    }
                }
            }
            finally
            {
                list.Dispose();
            }
        }

        /// <summary>
        /// Sorts the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        public static void Sort<T>(this NativeList<T> list) where T : unmanaged, IComparable<T>
        {
            Sort(list, 0, list.Length - 1, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void Sort<T>(this NativeList<T> list, int start, int end) where T : unmanaged, IComparable<T>
        {
            Sort(list, start, end, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void Sort<T>(this NativeList<T> list, int start, int end, IComparer<T> comparer) where T : unmanaged
        {
            if (!list.IsValid)
            {
                throw new ArgumentException("NativeList is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > list.Length || end > list.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            void* pointer = list._buffer;
            NativeSortUtilities.Sort(pointer, start, end, false, comparer);
        }

        /// <summary>
        /// Sorts the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="comparison">Used for compare the elements to sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        public static void Sort<T>(this NativeList<T> list, Comparison<T> comparison) where T : unmanaged
        {
            Sort(list, 0, list.Length - 1, comparison);
        }

        /// <summary>
        /// Sorts the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="comparison">Used for compare the elements to sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void Sort<T>(this NativeList<T> list, int start, int end, Comparison<T> comparison) where T : unmanaged
        {
            if (!list.IsValid)
            {
                throw new ArgumentException("NativeList is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > list.Length || end > list.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            void* pointer = list._buffer;
            NativeSortUtilities.Sort(pointer, start, end, false, comparison);
        }

        /// <summary>
        /// Sorts the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        public static void SortBy<T, TSelect>(this NativeList<T> list, Func<T, TSelect> selector) where T : unmanaged where TSelect : IComparable<TSelect>
        {
            SortBy(list, 0, list.Length - 1, Comparer<TSelect>.Default, selector);
        }

        /// <summary>
        /// Sorts the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="comparer">The comparer used for sorting</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        public static void SortBy<T, TSelect>(this NativeList<T> list, IComparer<TSelect> comparer, Func<T, TSelect> selector) where T : unmanaged
        {
            SortBy(list, 0, list.Length - 1, comparer, selector);
        }

        /// <summary>
        /// Sorts the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void SortBy<T, TSelect>(this NativeList<T> list, int start, int end, Func<T, TSelect> selector) where T : unmanaged where TSelect : IComparable<TSelect>
        {
            SortBy(list, start, end, Comparer<TSelect>.Default, selector);
        }

        /// <summary>
        /// Sorts the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="comparer">The comparer used for sorting</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void SortBy<T, TSelect>(this NativeList<T> list, int start, int end, IComparer<TSelect> comparer, Func<T, TSelect> selector) where T : unmanaged
        {
            if (!list.IsValid)
            {
                throw new ArgumentException("NativeList is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > list.Length || end > list.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            void* pointer = list._buffer;
            NativeSortUtilities.SortBy(pointer, start, end, false, comparer, selector);
        }

        /// <summary>
        /// Sorts by decending the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        public static void SortByDecending<T, TSelect>(this NativeList<T> list, Func<T, TSelect> selector) where T : unmanaged where TSelect : IComparable<TSelect>
        {
            SortByDecending(list, 0, list.Length - 1, Comparer<TSelect>.Default, selector);
        }

        /// <summary>
        /// Sorts by decending the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void SortByDecending<T, TSelect>(this NativeList<T> list, int start, int end, Func<T, TSelect> selector) where T : unmanaged where TSelect : IComparable<TSelect>
        {
            SortByDecending(list, start, end, Comparer<TSelect>.Default, selector);
        }

        /// <summary>
        /// Sorts by decending the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="comparer">The comparer used for sorting</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        public static void SortByDecending<T, TSelect>(this NativeList<T> list, IComparer<TSelect> comparer, Func<T, TSelect> selector) where T : unmanaged
        {
            SortByDecending(list, 0, list.Length - 1, comparer, selector);
        }

        /// <summary>
        /// Sorts by decending the content of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TSelect">The type of the key used for sorting</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="comparer">The comparer used for sorting</param>
        /// <param name="selector">The selects the key used for sort.</param>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static void SortByDecending<T, TSelect>(this NativeList<T> list, int start, int end, IComparer<TSelect> comparer, Func<T, TSelect> selector) where T : unmanaged
        {
            if (!list.IsValid)
            {
                throw new ArgumentException("NativeList is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > list.Length || end > list.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            void* pointer = list._buffer;
            NativeSortUtilities.SortBy(pointer, start, end, true, comparer, selector);
        }

        /// <summary>
        /// Finds the first element that matchs the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>The first element that matchs the condition or null if not found.</returns>
        public static T? FindFirst<T>(this NativeList<T> list, Predicate<T> predicate) where T : unmanaged
        {
            foreach (ref var e in list)
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
        /// <param name="list">The list.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>The last element that matchs the condition or null if not found.</returns>
        public static T? FindLast<T>(this NativeList<T> list, Predicate<T> predicate) where T : unmanaged
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (predicate(list[^i]))
                {
                    return list[^i];
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the first element that matchs the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>A reference to the first element that matchs the condition or null if not found.</returns>
        public static ref T? FindFirstRef<T>(this NativeList<T> list, Predicate<T> predicate) where T : unmanaged
        {
            foreach (ref var e in list)
            {
                if (predicate(e))
                {
                    return ref Unsafe.As<T, T?>(ref e);
                }
            }

            return ref UnsafeUtilities.NullRef<T?>();
        }

        /// <summary>
        /// Finds the last element that matchs the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>A reference to the last element that matchs the condition or null if not found.</returns>
        public static ref T? FindLastRef<T>(this NativeList<T> list, Predicate<T> predicate) where T : unmanaged
        {
            for(int i = 0; i < list.Length; i++)
            {
                if (predicate(list[^i]))
                {
                    return ref Unsafe.As<T, T?>(ref list[^i]);
                }
            }

            return ref UnsafeUtilities.NullRef<T?>();
        }

        /// <summary>
        /// Finds alls elements that match the specified predicate.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>An NativeArray with all the matches.</returns>
        public static NativeArray<T> FindAll<T>(this NativeList<T> list, Predicate<T> predicate) where T : unmanaged
        {
            int expected = Math.Min(1, list.Length / 2);
            NativeList<T> matches = new NativeList<T>(expected, list.GetAllocator()!);

            foreach (ref var e in list)
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
        /// <param name="list">The list.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>The index of the first element that meet the condition.</returns>
        public static int FindIndex<T>(this NativeList<T> list, Predicate<T> predicate) where T : unmanaged
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (predicate(list[i]))
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
        /// <param name="list">The list.</param>
        /// <param name="predicate">The predicate to use.</param>
        /// <returns>The index of the last element that meet the condition.</returns>
        public static int FindLastIndex<T>(this NativeList<T> list, Predicate<T> predicate) where T : unmanaged
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (predicate(list[^i]))
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
        /// <param name="list">The list.</param>
        /// <param name="predicate">The predicate used.</param>
        /// <returns><c>true</c> if a value exists; otherwise <c>false</c>.</returns>
        public static bool Exists<T>(this NativeList<T> list, Predicate<T> predicate) where T : unmanaged
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (predicate(list[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether all the elements in the list meet the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="predicate">The condition to use.</param>
        /// <returns>
        ///     <c>true</c> if all the elements meet the condition; otherwise <c>false</c>.
        /// </returns>
        public static bool TrueForAll<T>(this NativeList<T> list, Predicate<T> predicate) where T : unmanaged
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (!predicate(list[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Performs a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        public static int BinarySearch<T>(this NativeList<T> list, T value) where T : unmanaged, IComparable<T>
        {
            return BinarySearch(list, 0, list.Length - 1, value, Comparer<T>.Default);
        }

        /// <summary>
        /// Performs a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static int BinarySearch<T>(this NativeList<T> list, int start, int end, T value) where T : unmanaged, IComparable<T>
        {
            return BinarySearch(list, start, end, value, Comparer<T>.Default);
        }

        /// <summary>
        /// Performs a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="value">The value to find.</param>
        /// <param name="comparer">The comparer used to find the elements.</param>
        /// <returns>The index of the value or the index which the value should be as ~index.</returns>
        /// <exception cref="ArgumentException">If the list is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the specified range is invalid</exception>
        public static int BinarySearch<T>(this NativeList<T> list, int start, int end, T value, IComparer<T> comparer) where T : unmanaged
        {
            if (!list.IsValid)
            {
                throw new ArgumentException("NativeList is invalid");
            }

            if (start > end || start < 0 || end < 0 || start > list.Length || end > list.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid range, start: {start}, end: {end}");
            }

            return NativeCollectionUtilities.BinarySearch(list._buffer, start, end, value, comparer);
        }

        /// <summary>
        /// Performs a foreach loop over the elements of the given list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="action">The action to perform over each element.</param>
        public static void ForEach<T>(this NativeList<T> list, Action<T> action) where T : unmanaged
        {
            int length = list.Length;
            for (int i = 0; i < length; i++)
            {
                action(list[i]);
            }
        }

        /// <summary>
        /// Performs a foreach loop over the index-element values of the given list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="action">The action to perform over each element and its index.</param>
        public static void ForEach<T>(this NativeList<T> list, Action<T, int> action) where T : unmanaged
        {
            int length = list.Length;
            for (int i = 0; i < length; i++)
            {
                action(list[i], i);
            }
        }

        /// <summary>
        /// Performs a ref-like foreach loop over the elements of the list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="action">The action to perform over each reference of the elements.</param>
        public static void ForEachRef<T>(this NativeList<T> list, RefAction<T> action) where T : unmanaged
        {
            int length = list.Length;
            for (int i = 0; i < length; i++)
            {
                action(ref list[i]);
            }
        }

        /// <summary>
        /// Performs a ref-like foreach loop over the index-element values of the list.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="action">The action to perform over each reference of the elements and its index.</param>
        public static void ForEachRef<T>(this NativeList<T> list, RefIndexedAction<T> action) where T : unmanaged
        {
            int length = list.Length;
            for (int i = 0; i < length; i++)
            {
                action(ref list[i], i);
            }
        }
    }
}
