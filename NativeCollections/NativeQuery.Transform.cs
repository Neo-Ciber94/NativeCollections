using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public ref partial struct NativeQuery<T>
    {
        public NativeQuery<TResult> Select<TResult>(Func<T, TResult> selector) where TResult : unmanaged
        {
            if (_length == 0)
            {
                return default;
            }

            int length = _length;
            Allocator allocator = GetAllocator()!;
            TResult* buffer = GetAllocator()!.Allocate<TResult>(length);
            RefEnumerator<T> enumerable = GetEnumerator();

            if (enumerable.MoveNext())
            {
                for (int i = 0; i < length; i++)
                {
                    buffer[i] = selector(enumerable.Current);
                    enumerable.MoveNext();
                }
            }

            Dispose();
            return new NativeQuery<TResult>(buffer, length, allocator);
        }

        public NativeQuery<T> Where(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                return default;
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
                Dispose();
                return default;
            }

            Dispose();
            list.TrimExcess();
            return new NativeQuery<T>(list.GetUnsafePointer(), list.Length, allocator);
        }

        public NativeQuery<T> Take(int count)
        {
            if (_length == 0)
            {
                return default;
            }

            if (count < 0)
            {
                throw new ArgumentException(count.ToString(), nameof(count));
            }

            int length = count > _length ? _length : count;
            Allocator allocator = GetAllocator()!;
            void* src = _buffer;
            void* dst = allocator.Allocate<T>(length);
            Unsafe.CopyBlockUnaligned(dst, src, (uint)(sizeof(T) * count));
            Dispose();
            return new NativeQuery<T>(dst, count, allocator);
        }

        public NativeQuery<T> Skip(int count)
        {
            if (_length == 0)
            {
                return default;
            }

            if (count < 0)
            {
                throw new ArgumentException(count.ToString(), nameof(count));
            }

            int length = _length - count;

            if(length <= 0)
            {
                Dispose();
                return default;
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

        public NativeQuery<T> TakeWhile(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                return default;
            }

            Allocator allocator = GetAllocator()!;
            NativeList<T> list = new NativeList<T>(_length, allocator);
            RefEnumerator<T> enumerator = GetEnumerator();

            while (enumerator.MoveNext())
            {
                T t = enumerator.Current;

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
                Dispose();
                return default;
            }

            Dispose();
            list.TrimExcess();
            return new NativeQuery<T>(list.GetUnsafePointer(), list.Length, allocator);
        }

        public NativeQuery<T> SkipWhile(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                return default;
            }

            RefEnumerator<T> enumerator = GetEnumerator();
            bool hasNext = false;
            int skip = 0;

            if (enumerator.MoveNext())
            {
                while (true)
                {
                    hasNext = enumerator.MoveNext();
                    if (hasNext)
                    {
                        T t = enumerator.Current;
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
                }
            }

            if (hasNext is false)
            {
                Dispose();
                return default;
            }

            int length = _length - skip;
            if (length == 0)
            {
                Dispose();
                return default;
            }

            Allocator allocator = GetAllocator()!;
            NativeList<T> list = new NativeList<T>(length, allocator);

            do
            {
                T t = enumerator.Current;

                if (predicate(t))
                {
                    list.Add(t);
                }
                else
                {
                    break;
                }
            }
            while (enumerator.MoveNext());

            Dispose();
            return new NativeQuery<T>(list.GetUnsafePointer(), list.Length, allocator);
        }

        public NativeQuery<T> Reverse()
        {
            if (_length == 0)
            {
                return default;
            }

            UnsafeUtilities.Reverse<T>(_buffer, 0, _length);
            return this;
        }

        public NativeQuery<TResult> Cast<TResult>() where TResult : unmanaged
        {
            if (_length == 0)
            {
                return default;
            }

            if (_length % sizeof(TResult) == 0)
            {
                int d = _length / sizeof(TResult);
                int length = _length * d;
                return new NativeQuery<TResult>(_buffer, length, GetAllocator()!);
            }

            throw new InvalidCastException($"Cannot cast '{typeof(T)}' to '{typeof(TResult)}'.");
        }

        public NativeQuery<T> OrderBy<TSelect>(Func<T, TSelect> selector) where TSelect : unmanaged
        {
            return OrderBy(selector, Comparer<TSelect>.Default);
        }

        public NativeQuery<T> OrderBy<TSelect>(Func<T, TSelect> selector, IComparer<TSelect> comparer) where TSelect : unmanaged
        {
            if (_length == 0)
            {
                return default;
            }

            NativeSortUtilities.SortBy(_buffer, 0, _length - 1, false, comparer, selector);
            return this;
        }

        public NativeQuery<T> OrderByDecending<TSelect>(Func<T, TSelect> selector) where TSelect : unmanaged
        {
            return OrderByDecending(selector, Comparer<TSelect>.Default);
        }

        public NativeQuery<T> OrderByDecending<TSelect>(Func<T, TSelect> selector, IComparer<TSelect> comparer) where TSelect : unmanaged
        {
            if (_length == 0)
            {
                return default;
            }

            NativeSortUtilities.SortBy(_buffer, 0, _length - 1, true, comparer, selector);
            return this;
        }

        public NativeQuery<T> Seek(Action<T> action)
        {
            if (_length == 0)
            {
                return default;
            }

            foreach (var e in this)
            {
                action(e);
            }

            return this;
        }

        public NativeQuery<IndexedValue<T>> WithIndex()
        {
            if (_length == 0)
            {
                return default;
            }

            int length = _length;
            Allocator allocator = GetAllocator()!;
            IndexedValue<T>* buffer = GetAllocator()!.Allocate<IndexedValue<T>>(length);
            RefEnumerator<T> enumerable = GetEnumerator();

            if (enumerable.MoveNext())
            {
                for (int i = 0; i < length; i++)
                {
                    buffer[i] = new IndexedValue<T>(enumerable.Current, i);
                    enumerable.MoveNext();
                }
            }

            Dispose();
            return new NativeQuery<IndexedValue<T>>(buffer, length, allocator);
        }

        public NativeQuery<T> Distinct()
        {
            if(_length == 0)
            {
                return default;
            }

            Allocator allocator = GetAllocator()!;
            NativeSet<T> set = new NativeSet<T>(_length, allocator);
            foreach(ref var e in this)
            {
                set.Add(e);
            }

            NativeArray<T> array = set.ToNativeArrayAndDispose();
            Dispose();
            return new NativeQuery<T>(array.GetUnsafePointer(), array.Length, allocator);
        }
    }
}
