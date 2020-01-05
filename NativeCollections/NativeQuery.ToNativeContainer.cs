using System;

namespace NativeCollections
{
    unsafe public ref partial struct NativeQuery<T>
    {
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
            Dispose();
            return set;
        }

        public NativeStack<T> ToNativeStack()
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeStack<T> stack = new NativeStack<T>(_buffer, _length, GetAllocator()!);
            Dispose();
            return stack;
        }

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

            Dispose();
            return queue;
        }

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

            Dispose();
            return deque;
        }

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

            Dispose();
            return map;
        }

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

            Dispose();
            return map;
        }

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

            Dispose();
            return map;
        }

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

            Dispose();
            return map;
        }
    }
}
