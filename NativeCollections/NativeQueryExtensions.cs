using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    public static class NativeQueryExtensions
    {
        /// <summary>
        /// Gets the minimun value of this query and then dispose this query.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>The minimun value of the query.</returns>
        [DisposeAfterCall]
        public static T Min<T>(this NativeQuery<T> query) where T : unmanaged, IComparable<T>
        {
            T? min = null;

            foreach (ref var e in query)
            {
                if (min == null)
                {
                    min = e;
                }
                else
                {
                    int c = e.CompareTo(min.Value);
                    if (c < 0)
                    {
                        min = e;
                    }
                }
            }

            if (min == null)
            {
                throw new InvalidOperationException("Cannot find the min value.");
            }

            return min.Value;
        }

        /// <summary>
        /// Gets the maximun value of this query and then dispose this query.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>The maximun value of the query.</returns>
        [DisposeAfterCall]
        public static T Max<T>(this NativeQuery<T> query) where T : unmanaged, IComparable<T>
        {
            T? max = null;

            foreach (ref var e in query)
            {
                if (max == null)
                {
                    max = e;
                }
                else
                {
                    int c = e.CompareTo(max.Value);
                    if (c > 0)
                    {
                        max = e;
                    }
                }
            }

            if (max == null)
            {
                throw new InvalidOperationException("Cannot find the min value.");
            }

            return max.Value;
        }

        /// <summary>
        /// Gets the minimun value of this query and then dispose this query.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="comparison">Provides a comparison of 2 values.</param>
        /// <returns>The minimun value of the query.</returns>
        [DisposeAfterCall]
        public static T Min<T>(this NativeQuery<T> query, Comparison<T> comparison) where T: unmanaged
        {
            T? min = null;

            foreach (ref var e in query)
            {
                if (min == null)
                {
                    min = e;
                }
                else
                {
                    int c = comparison(e, min.Value);
                    if (c < 0)
                    {
                        min = e;
                    }
                }
            }

            if (min == null)
            {
                throw new InvalidOperationException("Cannot find the min value.");
            }

            return min.Value;
        }

        /// <summary>
        /// Gets the maximun value of this query and then dispose this query.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="comparison">Provides a comparison of 2 values.</param>
        /// <returns>The maximun value of the query.</returns>
        [DisposeAfterCall]
        public static T Max<T>(this NativeQuery<T> query, Comparison<T> comparison) where T : unmanaged
        {
            T? max = null;

            foreach (ref var e in query)
            {
                if (max == null)
                {
                    max = e;
                }
                else
                {
                    int c = comparison(e, max.Value);
                    if (c > 0)
                    {
                        max = e;
                    }
                }
            }

            if (max == null)
            {
                throw new InvalidOperationException("Cannot find the min value.");
            }

            return max.Value;
        }

        /// <summary>
        /// Sums the values of this query and gets the <see cref="int"/> result, then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The sum of all the values of the query.</returns>
        /// <exception cref="OverflowException">If the result is greater than <c>MaxValue</c> or lower than <c>MinValue</c>.</exception>
        [DisposeAfterCall]
        public static int Sum(this NativeQuery<int> query)
        {
            int total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        /// <summary>
        /// Sums the values of this query and gets the <see cref="long"/> result, then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The sum of all the values of the query.</returns>
        /// <exception cref="OverflowException">If the result is greater than <c>MaxValue</c> or lower than <c>MinValue</c>.</exception>
        [DisposeAfterCall]
        public static long Sum(this NativeQuery<long> query)
        {
            long total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        /// <summary>
        /// Sums the values of this query and gets the <see cref="uint"/> result, then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The sum of all the values of the query.</returns>
        /// <exception cref="OverflowException">If the result is greater than <c>MaxValue</c> or lower than <c>MinValue</c>.</exception>
        [DisposeAfterCall]
        public static uint Sum(this NativeQuery<uint> query)
        {
            uint total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        /// <summary>
        /// Sums the values of this query and gets the <see cref="ulong"/> result, then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The sum of all the values of the query.</returns>
        /// <exception cref="OverflowException">If the result is greater than <c>MaxValue</c> or lower than <c>MinValue</c>.</exception>
        [DisposeAfterCall]
        public static ulong Sum(this NativeQuery<ulong> query)
        {
            ulong total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        /// <summary>
        /// Sums the values of this query and gets the <see cref="float"/> result, then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The sum of all the values of the query.</returns>
        /// <exception cref="OverflowException">If the result is greater than <c>MaxValue</c> or lower than <c>MinValue</c>.</exception>
        [DisposeAfterCall]
        public static float Sum(this NativeQuery<float> query)
        {
            float total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        /// <summary>
        /// Sums the values of this query and gets the <see cref="double"/> result, then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The sum of all the values of the query.</returns>
        /// <exception cref="OverflowException">If the result is greater than <c>MaxValue</c> or lower than <c>MinValue</c>.</exception>
        [DisposeAfterCall]
        public static double Sum(this NativeQuery<double> query)
        {
            double total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        /// <summary>
        /// Sums the values of this query and gets the <see cref="decimal"/> result, then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The sum of all the values of the query.</returns>
        /// <exception cref="OverflowException">If the result is greater than <c>MaxValue</c> or lower than <c>MinValue</c>.</exception>
        [DisposeAfterCall]
        public static decimal Sum(this NativeQuery<decimal> query)
        {
            decimal total = 0;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total;
        }

        /// <summary>
        /// Gets the average <see cref="int"/> value of this query and then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The average value of this query.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty.</exception>
        [DisposeAfterCall]
        public static int Average(this NativeQuery<int> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            int total = 0;
            int length = query.Length;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / length;
        }

        /// <summary>
        /// Gets the average <see cref="long"/> value of this query and then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The average value of this query.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty.</exception>
        [DisposeAfterCall]
        public static long Average(this NativeQuery<long> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            long total = 0;
            int length = query.Length;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / length;
        }

        /// <summary>
        /// Gets the average <see cref="uint"/> value of this query and then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The average value of this query.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty.</exception>
        [DisposeAfterCall]
        public static uint Average(this NativeQuery<uint> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            uint total = 0;
            int length = query.Length;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / (uint)length;
        }

        /// <summary>
        /// Gets the average <see cref="ulong"/> value of this query and then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The average value of this query.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty.</exception>
        [DisposeAfterCall]
        public static ulong Average(this NativeQuery<ulong> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            ulong total = 0;
            int length = query.Length;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / (ulong)length;
        }

        /// <summary>
        /// Gets the average <see cref="float"/> value of this query and then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The average value of this query.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty.</exception>
        [DisposeAfterCall]
        public static float Average(this NativeQuery<float> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            float total = 0;
            int length = query.Length;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / length;
        }

        /// <summary>
        /// Gets the average <see cref="double"/> value of this query and then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The average value of this query.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty.</exception>
        [DisposeAfterCall]
        public static double Average(this NativeQuery<double> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            double total = 0;
            int length = query.Length;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / length;
        }

        /// <summary>
        /// Gets the average <see cref="decimal"/> value of this query and then dispose this query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The average value of this query.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty.</exception>
        [DisposeAfterCall]
        public static decimal Average(this NativeQuery<decimal> query)
        {
            if (query.IsEmpty)
            {
                throw new InvalidOperationException("NativeQuery is empty.");
            }

            decimal total = 0;
            int length = query.Length;
            foreach (ref var e in query)
            {
                total += e;
            }

            return total / length;
        }

        /// <summary>
        /// Sorts the elements of this query.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>This query with the elements sorted.</returns>
        unsafe public static NativeQuery<T> Sorted<T>(this NativeQuery<T> query) where T : unmanaged, IComparable<T>
        {
            if (query.IsEmpty)
            {
                return new NativeQuery<T>(query.GetAllocator());
            }

            NativeSortUtilities.Sort<T>(query._buffer, query.Length);
            return query;
        }

        /// <summary>
        /// Sorts the elements of this query.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>This query with the elements sorted.</returns>
        unsafe public static NativeQuery<T> Sorted<T>(this NativeQuery<T> query, IComparer<T> comparer) where T : unmanaged
        {
            if (query.IsEmpty)
            {
                return new NativeQuery<T>(query.GetAllocator());
            }

            NativeSortUtilities.Sort(query._buffer, query.Length, comparer);
            return query;
        }

        /// <summary>
        /// Sorts the elements of this query.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns>This query with the elements sorted.</returns>
        unsafe public static NativeQuery<T> Sorted<T>(this NativeQuery<T> query, Comparison<T> comparison) where T : unmanaged
        {
            if (query.IsEmpty)
            {
                return new NativeQuery<T>(query.GetAllocator());
            }

            NativeSortUtilities.Sort(query._buffer, query.Length, comparison);
            return query;
        }

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> with the default value if this query is empty, otherwise get the same query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A query with the default value if empty, otherwise the same query will be returned.</returns>
        public static NativeQuery<T> DefaultIfEmpty<T>(this NativeQuery<T> query, T defaultValue) where T : unmanaged
        {
            if (query.IsEmpty)
            {
                unsafe
                {
                    Allocator allocator = query.GetAllocator() ?? Allocator.Default;
                    T* value = allocator.Allocate<T>(1);
                    *value = defaultValue;
                    return new NativeQuery<T>(value, 1, allocator);
                }
            }

            return query;
        }

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> with the default values if this query is empty, otherwise get the same query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="defaultValues">The default values.</param>
        /// <returns>A query with the default value if empty, otherwise the same query will be returned.</returns>
        public static NativeQuery<T> DefaultIfEmpty<T>(this NativeQuery<T> query, Span<T> defaultValues) where T : unmanaged
        {
            if (query.IsEmpty)
            {
                if (defaultValues.IsEmpty)
                {
                    return new NativeQuery<T>(query.GetAllocator());
                }

                unsafe
                {
                    int length = defaultValues.Length;
                    Allocator allocator = query.GetAllocator() ?? Allocator.Default;
                    T* buffer = allocator.Allocate<T>(length);
                    void* src = Unsafe.AsPointer(ref defaultValues[0]);
                    Unsafe.CopyBlockUnaligned(buffer, src, (uint)(sizeof(T) * length));
                    return new NativeQuery<T>(buffer, length, allocator);
                }
            }

            return query;
        }

        /// <summary>
        /// Gets a new <see cref="NativeQuery{T}"/> which elements are the result of operate with the elements of this
        /// query and the specified <see cref="Span{T}"/>.
        /// </summary>
        /// <typeparam name="TFirst">The type of the first element.</typeparam>
        /// <typeparam name="TSecond">The type of the second element.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="elements">The elements.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The resulting query.</returns>
        unsafe public static NativeQuery<TResult> Zip<TFirst, TSecond, TResult>(this NativeQuery<TFirst> query, in Span<TSecond> elements, Func<TFirst, TSecond, TResult> operation) where TFirst : unmanaged where TSecond : unmanaged where TResult : unmanaged
        {
            if(query.IsEmpty || elements.IsEmpty)
            {
                var emptyQuery = new NativeQuery<TResult>(query.GetAllocator());
                query.Dispose();
                return emptyQuery;
            }

            int length = Math.Min(query.Length, elements.Length);
            NativeArray<TResult> result = new NativeArray<TResult>(length);

            for (int i = 0; i < length; i++)
            {
                TFirst first = query[i];
                TSecond second = elements[i];
                result[i] = operation(first, second);
            }

            var allocator = query.GetAllocator()!;
            query.Dispose();
            return new NativeQuery<TResult>(result.GetUnsafePointer(), result.Length, allocator);
        }

        /// <summary>
        /// Releases the resources used for this query and each of its elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="disposing">if set to <c>true</c> will call dispose for each element in the query.</param>
        public static void Dispose<T>(this ref NativeQuery<T> query, bool disposing) where T : unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (ref var e in query)
                    {
                        e.Dispose();
                    }
                }
            }
            finally
            {
                query.Dispose();
            }
        }
        
        /// <summary>
        /// Concatenates the elements of this query with the given <see cref="Span{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="elements">The elements.</param>
        /// <returns>A new query with the concatenates elements.</returns>
        unsafe public static NativeQuery<T> Concat<T>(this NativeQuery<T> query, in Span<T> elements) where T: unmanaged
        {
            if (query.Length == 0 && elements.IsEmpty)
            {
                return query;
            }

            NativeList<T> list = new NativeList<T>(query.Length + elements.Length);
            list.AddAll(query._buffer, query.Length);
            list.AddAll(elements);

            Allocator allocator = query.GetAllocator()!;
            query.Dispose();
            return new NativeQuery<T>(list.GetUnsafePointer(), list.Length, allocator);
        }

        /// <summary>
        /// Concatenates the elements of this query with the given <see cref="Span{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="elements">The elements.</param>
        /// <returns>A new query with the concatenates elements.</returns>
        unsafe public static NativeQuery<T> Concat<T>(this NativeQuery<T> query, in ReadOnlySpan<T> elements) where T : unmanaged
        {
            if (query.Length == 0 && elements.IsEmpty)
            {
                return query;
            }

            NativeList<T> list = new NativeList<T>(query.Length + elements.Length);
            list.AddAll(query._buffer, query.Length);
            list.AddAll(elements);

            Allocator allocator = query.GetAllocator()!;
            query.Dispose();
            return new NativeQuery<T>(list.GetUnsafePointer(), list.Length, allocator);
        }

        /// <summary>
        /// Determines if this query and the elements have the same sequence of elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="elements">The elements to compare to.</param>
        /// <returns><c>true</c> if the sequence are equals, otherwise <c>false</c>.</returns>
        unsafe public static bool SequenceEquals<T>(this NativeQuery<T> query, in Span<T> elements) where T : unmanaged
        {
            if (query.Length != elements.Length)
            {
                query.Dispose();
                return false;
            }

            var comparer = EqualityComparer<T>.Default;
            bool result = true;

            for (int i = 0; i < query.Length; i++)
            {
                if (comparer.Equals(query._buffer[i], elements[i]) is false)
                {
                    result = false;
                    break;
                }
            }

            query.Dispose();
            return result;
        }

        /// <summary>
        /// Determines if this query and the elements have the same sequence of elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="elements">The elements to compare to.</param>
        /// <returns><c>true</c> if the sequence are equals, otherwise <c>false</c>.</returns>
        unsafe public static bool SequenceEquals<T>(this NativeQuery<T> query, in ReadOnlySpan<T> elements) where T : unmanaged
        {
            if (query.Length != elements.Length)
            {
                query.Dispose();
                return false;
            }

            var comparer = EqualityComparer<T>.Default;
            bool result = true;

            for (int i = 0; i < query.Length; i++)
            {
                if (comparer.Equals(query._buffer[i], elements[i]) is false)
                {
                    result = false;
                    break;
                }
            }

            query.Dispose();
            return result;
        }
    }
}
