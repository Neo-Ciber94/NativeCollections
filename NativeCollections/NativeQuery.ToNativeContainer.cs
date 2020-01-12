using System;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public ref partial struct NativeQuery<T>
    {
        /// <summary>
        /// Gets a <see cref="NativeArray{T}"/> using the elements of this query and then dispose this instance.
        /// </summary>
        /// <returns>A native array with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeArray<T> ToNativeArray()
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeArray<T> array = new NativeArray<T>(_buffer, _length, GetAllocator()!);
            this = default;
            return array;
        }

        /// <summary>
        /// Gets a <see cref="NativeList{T}"/> using the elements of this query and then dispose this instance.
        /// </summary>
        /// <returns>A native list with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeList<T> ToNativeList()
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeList<T> array = new NativeList<T>(_buffer, _length, GetAllocator()!);
            this = default;
            return array;
        }

        /// <summary>
        /// Gets a <see cref="NativeSet{T}"/> using the elements of this query and then dispose this instance.
        /// </summary>
        /// <returns>A native set with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeSet<T> ToNativeSet()
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeSet<T> set = new NativeSet<T>(_length, GetAllocator()!);
            foreach (ref var e in this)
            {
                set.Add(e);
            }

            set.TrimExcess();
            return set;
        }

        /// <summary>
        /// Gets a <see cref="NativeSet{T}"/> using the elements of this query and then dispose this instance.
        /// </summary>
        /// <returns>A native set with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeStack<T> ToNativeStack()
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeStack<T> stack = new NativeStack<T>(_buffer, _length, GetAllocator()!);
            this = default;
            return stack;
        }

        /// <summary>
        /// Gets a <see cref="NativeQueue{T}"/> using the elements of this query and then dispose this instance.
        /// </summary>
        /// <returns>A native queue with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeQueue<T> ToNativeQueue()
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeQueue<T> queue = new NativeQueue<T>(_length, GetAllocator()!);
            foreach (ref var e in this)
            {
                queue.Enqueue(e);
            }

            return queue;
        }

        /// <summary>
        /// Gets a <see cref="NativeDeque{T}"/> using the elements of this query and then dispose this instance.
        /// </summary>
        /// <returns>A native deque with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeDeque<T> ToNativeDeque()
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeDeque<T> deque = new NativeDeque<T>(_length, GetAllocator()!);
            foreach (ref var e in this)
            {
                deque.AddFirst(e);
            }

            return deque;
        }

        /// <summary>
        /// Gets a <see cref="NativeSortedSet{T}"/> using the elements of this query and then dispose this instance.
        /// </summary>
        /// <returns>A native set with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeSortedSet<T> ToNativeSortedSet()
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeSortedSet<T> set = new NativeSortedSet<T>(_length, GetAllocator()!);
            foreach (ref var e in this)
            {
                set.Add(e);
            }

            set.TrimExcess();
            return set;
        }

        /// <summary>
        /// Gets a <see cref="NativeMap{TKey, TValue}"/> using the elements of this query and then dispose this instance.
        /// Each repeated key will be ignore.
        /// </summary>
        /// <param name="keySelector">Provides the keys of the map.</param>
        /// <returns>A native map with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeMap<TKey, T> ToNativeMap<TKey>(Func<T, TKey> keySelector) where TKey : unmanaged
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeMap<TKey, T> map = new NativeMap<TKey, T>(_length, GetAllocator()!);
            foreach (ref var e in this)
            {
                TKey key = keySelector(e);
                map.TryAdd(key, e);
            }

            return map;
        }

        /// <summary>
        /// Gets a <see cref="NativeMap{TKey, TValue}"/> using the elements of this query and then dispose this instance.
        /// Each repeated key will be ignore.
        /// </summary>
        /// <param name="keySelector">Provides the keys of the map.</param>
        /// <param name="valueSelector">Provides the values of the map.</param>
        /// <returns>A native map with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeMap<TKey, TValue> ToNativeMap<TKey, TValue>(Func<T, TKey> keySelector, Func<T, TValue> valueSelector) where TKey : unmanaged where TValue : unmanaged
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeMap<TKey, TValue> map = new NativeMap<TKey, TValue>(_length, GetAllocator()!);
            foreach (ref var e in this)
            {
                TKey key = keySelector(e);
                TValue value = valueSelector(e);
                map.TryAdd(key, value);
            }

            return map;
        }

        /// <summary>
        /// Gets a <see cref="NativeSortedMap{TKey, TValue}"/> using the elements of this query and then dispose this instance.
        /// Each repeated key will be ignore.
        /// </summary>
        /// <param name="keySelector">Provides the keys of the map.</param>
        /// <returns>A native map with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeSortedMap<TKey, T> ToNativeSortedMap<TKey>(Func<T, TKey> keySelector) where TKey : unmanaged
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeSortedMap<TKey, T> map = new NativeSortedMap<TKey, T>(_length, GetAllocator()!);
            foreach (ref var e in this)
            {
                TKey key = keySelector(e);
                map.TryAdd(key, e);
            }

            return map;
        }

        /// <summary>
        /// Gets a <see cref="NativeSortedMap{TKey, TValue}"/> using the elements of this query and then dispose this instance.
        /// Each repeated key will be ignore.
        /// </summary>
        /// <param name="keySelector">Provides the keys of the map.</param>
        /// <param name="valueSelector">Provides the values of the map.</param>
        /// <returns>A native map with this instance elements.</returns>
        [DisposeAfterCall]
        public NativeSortedMap<TKey, TValue> ToNativeSortedMap<TKey, TValue>(Func<T, TKey> keySelector, Func<T, TValue> valueSelector) where TKey : unmanaged where TValue : unmanaged
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeSortedMap<TKey, TValue> map = new NativeSortedMap<TKey, TValue>(_length, GetAllocator()!);
            foreach (ref var e in this)
            {
                TKey key = keySelector(e);
                TValue value = valueSelector(e);
                map.TryAdd(key, value);
            }

            return map;
        }
    }
}
