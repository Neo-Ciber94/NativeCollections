using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public ref partial struct NativeQuery<T>
    {
        /// <summary>
        /// Maps each element of the query into a new value and then dispose this instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="selector">The provides the new value.</param>
        /// <returns>The mapped query.</returns>
        [DisposeAfterCall]
        public NativeQuery<TResult> Select<TResult>(Func<T, TResult> selector) where TResult : unmanaged
        {
            if (_length == 0)
            {
                return new NativeQuery<TResult>(GetAllocator());
            }

            int length = _length;
            Allocator allocator = GetAllocator()!;
            TResult* buffer = GetAllocator()!.Allocate<TResult>(length);
            Enumerator enumerator = GetEnumerator(disposing: false);

            if (enumerator.MoveNext())
            {
                for (int i = 0; i < length; i++)
                {
                    buffer[i] = selector(enumerator.Current);
                    enumerator.MoveNext();
                }
            }

            Dispose();
            return new NativeQuery<TResult>(buffer, length, allocator);
        }

        /// <summary>
        /// Filters the content of this query and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The condition to filter the elements.</param>
        /// <returns>The filtered query.</returns>
        [DisposeAfterCall]
        public NativeQuery<T> Where(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            Allocator allocator = GetAllocator()!;
            NativeList<T> list = new NativeList<T>(_length, allocator);

            foreach (ref var e in this)
            {
                if (predicate(e))
                {
                    list.Add(e);
                }
            }

            if (list.Length == 0)
            {
                list.Dispose();
                var emptyQuery = new NativeQuery<T>(GetAllocator());
                Dispose();
                return emptyQuery;
            }

            Dispose();
            list.TrimExcess();
            return new NativeQuery<T>(list.GetUnsafePointer(), list.Length, allocator);
        }

        /// <summary>
        /// Takes the first <c>n</c> element of this query and then dispose this instance.
        /// </summary>
        /// <param name="count">The number of elements to take.</param>
        /// <returns>The new query with the resulting elements.</returns>
        [DisposeAfterCall]
        public NativeQuery<T> Take(int count)
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            if(count == 0)
            {
                var emptyQuery = new NativeQuery<T>(GetAllocator());
                Dispose();
                return emptyQuery;
            }

            if (count < 0)
            {
                Dispose();
                throw new ArgumentException(count.ToString(), nameof(count));
            }

            int length = count >= _length ? _length : count;
            Allocator allocator = GetAllocator()!;
            void* src = _buffer;
            void* dst = allocator.Allocate<T>(length);
            Unsafe.CopyBlockUnaligned(dst, src, (uint)(sizeof(T) * count));
            Dispose();
            return new NativeQuery<T>(dst, count, allocator);
        }

        /// <summary>
        /// Skips the first <c>n</c> element of this query and then dispose this instance.
        /// </summary>
        /// <param name="count">The number of elements to skip.</param>
        /// <returns>The new query with the resulting elements.</returns>
        [DisposeAfterCall]
        public NativeQuery<T> Skip(int count)
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            if (count < 0)
            {
                Dispose();
                throw new ArgumentException(count.ToString(), nameof(count));
            }

            int length = _length - count;

            if(length <= 0)
            {
                var emptyQuery = new NativeQuery<T>(GetAllocator());
                Dispose();
                return emptyQuery;
            }
            else
            {
                Allocator allocator = GetAllocator()!;
                void* src = _buffer + count;
                void* dst = allocator.Allocate<T>(length);
                Unsafe.CopyBlockUnaligned(dst, src, (uint)(sizeof(T) * length));
                Dispose();
                return new NativeQuery<T>(dst, length, allocator);
            }
        }

        /// <summary>
        /// Takes elements from this query while the condition is true and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The condition.</param>
        /// <returns>The new query with the resulting elements.</returns>
        [DisposeAfterCall]
        public NativeQuery<T> TakeWhile(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            Allocator allocator = GetAllocator()!;
            NativeList<T> list = new NativeList<T>(_length, allocator);
            Enumerator enumerator = GetEnumerator(disposing: false);

            while (enumerator.MoveNext())
            {
                ref T t = ref enumerator.Current;

                if (predicate(t))
                {
                    list.Add(t);
                }
                else
                {
                    break;
                }
            }

            if (list.Length == 0)
            {
                list.Dispose();
                var emptyQuery = new NativeQuery<T>(GetAllocator());
                Dispose();
                return emptyQuery;
            }

            Dispose();
            list.TrimExcess();
            return new NativeQuery<T>(list.GetUnsafePointer(), list.Length, allocator);
        }

        /// <summary>
        /// Skips elements from this query while the condition is true and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The condition.</param>
        /// <returns>The new query with the resulting elements.</returns>
        [DisposeAfterCall]
        public NativeQuery<T> SkipWhile(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            Enumerator enumerator = GetEnumerator(disposing: false);
            bool hasNext = enumerator.MoveNext();
            int skip = 0;

            while (hasNext)
            {
                if (hasNext)
                {
                    ref T t = ref enumerator.Current;
                    if (!predicate(t))
                    {
                        break;
                    }

                    ++skip;
                }
                else
                {
                    break;
                }

                hasNext = enumerator.MoveNext();
            }

            if (hasNext is false)
            {
                var emptyQuery = new NativeQuery<T>(GetAllocator());
                Dispose();
                return emptyQuery;
            }

            int length = _length - skip;
            if (length == 0)
            {
                var emptyQuery = new NativeQuery<T>(GetAllocator());
                Dispose();
                return emptyQuery;
            }

            Allocator allocator = GetAllocator()!;
            NativeList<T> list = new NativeList<T>(length, allocator);

            do
            {
                ref T t = ref enumerator.Current;
                list.Add(t);
            }
            while (enumerator.MoveNext());

            Dispose();
            return new NativeQuery<T>(list.GetUnsafePointer(), list.Length, allocator);
        }

        /// <summary>
        /// Takes the specified number of elements from the end of this query.
        /// </summary>
        /// <param name="count">Number of elements to take.</param>
        /// <returns>The new query with the resulting elements.</returns>
        [DisposeAfterCall]
        public NativeQuery<T> TakeLast(int count)
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            if (count == 0)
            {
                var emptyQuery = new NativeQuery<T>(GetAllocator());
                Dispose();
                return emptyQuery;
            }

            if (count < 0)
            {
                Dispose();
                throw new ArgumentException(count.ToString(), nameof(count));
            }

            int length = count >= _length ? _length : count;
            Allocator allocator = GetAllocator()!;
            void* src = _buffer + (_length - count);
            void* dst = allocator.Allocate<T>(length);
            Unsafe.CopyBlockUnaligned(dst, src, (uint)(sizeof(T) * length));
            Dispose();
            return new NativeQuery<T>(dst, length, allocator);
        }

        /// <summary>
        /// Skips elements from the end of this query.
        /// </summary>
        /// <param name="count">Number of elements to skip.</param>
        /// <returns>The new query with the resulting elements.</returns>
        [DisposeAfterCall]
        public NativeQuery<T> SkipLast(int count)
        {
            if (_length == 0 || count > _length)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            if (count < 0)
            {
                Dispose();
                throw new ArgumentException(count.ToString(), nameof(count));
            }

            int length = _length - count;

            Allocator allocator = GetAllocator()!;
            void* src = _buffer;
            void* dst = allocator.Allocate<T>(length);
            Unsafe.CopyBlockUnaligned(dst, src, (uint)(sizeof(T) * length));
            Dispose();
            return new NativeQuery<T>(dst, length, allocator);
        }

        /// <summary>
        /// Reverses the order of the elements of this query.
        /// </summary>
        /// <returns>This query with the elements in reverse order.</returns>
        public NativeQuery<T> Reverse()
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            UnsafeUtilities.Reverse<T>(_buffer, 0, _length - 1);
            return this;
        }

        /// <summary>
        /// Casts this instance to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type to cast to.</typeparam>
        /// <returns>This query casted into the given type</returns>
        [DisposeAfterCall]
        public NativeQuery<TResult> Cast<TResult>() where TResult : unmanaged
        {
            if (_length == 0)
            {
                return new NativeQuery<TResult>(GetAllocator());
            }

            if (sizeof(T) % sizeof(TResult) == 0)
            {
                int d = sizeof(T) / sizeof(TResult);
                int length = _length * d;
                NativeQuery<TResult> result = new NativeQuery<TResult>(_buffer, length, GetAllocator()!);
                this = default;
                return result;
            }

            Dispose();
            throw new InvalidCastException($"Cannot cast '{typeof(T)}' to '{typeof(TResult)}'.");
        }

        /// <summary>
        /// Sorts the elements of this query by using the values of the selector.
        /// </summary>
        /// <typeparam name="TSelect">The type of the select.</typeparam>
        /// <param name="selector">Provides the values used for sort the query.</param>
        /// <returns>This query with the elements sorted.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeQuery<T> OrderBy<TSelect>(Func<T, TSelect> selector) where TSelect : unmanaged, IComparable<TSelect>
        {
            return OrderBy(selector, Comparer<TSelect>.Default);
        }

        /// <summary>
        /// Sorts the elements of this query by using the values of the selector and the <see cref="IComparer{T}"/>.
        /// </summary>
        /// <typeparam name="TSelect">The type of the select.</typeparam>
        /// <param name="selector">Provides the values used for sort the query.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>This query with the elements sorted.</returns>
        public NativeQuery<T> OrderBy<TSelect>(Func<T, TSelect> selector, IComparer<TSelect> comparer) where TSelect : unmanaged
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            NativeSortUtilities.SortBy(_buffer, 0, _length - 1, false, comparer, selector);
            return this;
        }

        /// <summary>
        /// Sorts the elements of this query by decending using the values of the selector.
        /// </summary>
        /// <typeparam name="TSelect">The type of the select.</typeparam>
        /// <param name="selector">Provides the values used for sort the query.</param>
        /// <returns>This query with the elements sorted.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeQuery<T> OrderByDecending<TSelect>(Func<T, TSelect> selector) where TSelect : unmanaged, IComparable<TSelect>
        {
            return OrderByDecending(selector, Comparer<TSelect>.Default);
        }

        /// <summary>
        /// Sorts the elements of this query by decending using the values of the selector and the <see cref="IComparer{T}"/>.
        /// </summary>
        /// <typeparam name="TSelect">The type of the select.</typeparam>
        /// <param name="selector">Provides the values used for sort the query.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>This query with the elements sorted.</returns>
        public NativeQuery<T> OrderByDecending<TSelect>(Func<T, TSelect> selector, IComparer<TSelect> comparer) where TSelect : unmanaged
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            NativeSortUtilities.SortBy(_buffer, 0, _length - 1, true, comparer, selector);
            return this;
        }

        /// <summary>
        /// Performs the given <see cref="Action{T}"/> over each of the elements of the query.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>This instance.</returns>
        public NativeQuery<T> Seek(Action<T> action)
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            foreach (var e in this)
            {
                action(e);
            }

            return this;
        }

        /// <summary>
        /// Maps this instance into a <see cref="IndexedValue{T}"/> query.
        /// </summary>
        /// <returns>A new query with indexed values.</returns>
        [DisposeAfterCall]
        public NativeQuery<IndexedValue<T>> WithIndex()
        {
            if (_length == 0)
            {
                return new NativeQuery<IndexedValue<T>>(GetAllocator());
            }

            int length = _length;
            Allocator allocator = GetAllocator()!;
            IndexedValue<T>* buffer = GetAllocator()!.Allocate<IndexedValue<T>>(length);
            Enumerator enumerator = GetEnumerator(disposing: false);

            if (enumerator.MoveNext())
            {
                for (int i = 0; i < length; i++)
                {
                    buffer[i] = new IndexedValue<T>(enumerator.Current, i);
                    enumerator.MoveNext();
                }
            }

            Dispose();
            return new NativeQuery<IndexedValue<T>>(buffer, length, allocator);
        }

        /// <summary>
        /// Gets a query with all the distinct elements and then dispose this query.
        /// </summary>
        /// <returns>A query with distinct elements.</returns>
        [DisposeAfterCall]
        public NativeQuery<T> Distinct()
        {
            if (_length == 0)
            {
                return new NativeQuery<T>(GetAllocator());
            }

            Allocator allocator = GetAllocator()!;
            NativeSet<T> set = new NativeSet<T>(_length, allocator);
            foreach (ref var e in this)
            {
                set.Add(e);
            }

            set.TrimExcess();
            NativeArray<T> array = set.ToNativeArrayAndDispose();
            Dispose();
            return new NativeQuery<T>(array.GetUnsafePointer(), array.Length, allocator);
        }

        /// <summary>
        /// Appends the specified value to the query.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A query with the value appended.</returns>
        [DisposeAfterCall]
        public NativeQuery<T> Append(T value)
        {
            Allocator? allocator = GetAllocator()!;

            if(allocator == null)
            {
                return default;
            }

            NativeList<T> list = new NativeList<T>(_length + 1, allocator);
            list.AddAll(_buffer, _length);
            list.Add(value);

            Dispose();
            return new NativeQuery<T>(list.GetUnsafePointer(), list.Length, allocator);
        }

        /// <summary>
        /// Prepends the specified value to the query.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A query with the value prepended.</returns>
        [DisposeAfterCall]
        public NativeQuery<T> Prepend(T value)
        {
            Allocator? allocator = GetAllocator();

            if (allocator == null)
            {
                return default;
            }

            NativeList<T> list = new NativeList<T>(_length + 1, allocator);
            list.Add(value);
            list.AddAll(_buffer, _length);

            Dispose();
            return new NativeQuery<T>(list.GetUnsafePointer(), list.Length, allocator);
        }
    }
}
