using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NativeCollections.Allocators;

namespace NativeCollections
{
    unsafe public static class NativeQueryHelper
    {
        private static void* AllocateCopy<T>(void* pointer, int elementCount, Allocator allocator) where T: unmanaged
        {
            if(pointer == null)
            {
                throw new ArgumentException("pointer is null");
            }

            if (elementCount <= 0)
            {
                throw new ArgumentException($"elementCount cannot be negative or 0: {elementCount}");
            }

            void* destination = allocator.Allocate<T>(elementCount);
            Unsafe.CopyBlockUnaligned(destination, pointer, (uint)(sizeof(T) * elementCount));
            return destination;
        }

        public static NativeQuery<T> AsQuery<T>(this NativeArray<T> array) where T: unmanaged
        {
            if (array.IsEmpty)
            {
                return default;
            }

            Allocator allocator = array.GetAllocator()!;
            void* buffer = AllocateCopy<T>(array.GetUnsafePointer(), array.Length, allocator);
            return new NativeQuery<T>(buffer, array.Length, allocator);
        }

        public static NativeQuery<T> AsQuery<T>(this NativeList<T> list) where T : unmanaged
        {
            if (list.IsEmpty)
            {
                return default;
            }

            Allocator allocator = list.GetAllocator()!;
            void* buffer = AllocateCopy<T>(list.GetUnsafePointer(), list.Length, allocator);
            return new NativeQuery<T>(buffer, list.Length, allocator);
        }

        public static NativeQuery<T> AsQuery<T>(this NativeStack<T> stack) where T : unmanaged
        {
            if (stack.IsEmpty)
            {
                return default;
            }

            Allocator allocator = stack.GetAllocator()!;
            void* buffer = AllocateCopy<T>(stack.GetUnsafePointer(), stack.Length, allocator);
            return new NativeQuery<T>(buffer, stack.Length, allocator);
        }

        public static NativeQuery<T> AsQuery<T>(this NativeQueue<T> queue) where T : unmanaged
        {
            if (queue.IsEmpty)
            {
                return default;
            }

            Allocator allocator = queue.GetAllocator()!;
            NativeArray<T> array = queue.ToNativeArray();
            return new NativeQuery<T>(array.GetUnsafePointer(), queue.Length, allocator);
        }

        public static NativeQuery<T> AsQuery<T>(this NativeDeque<T> deque) where T : unmanaged
        {
            if (deque.IsEmpty)
            {
                return default;
            }

            Allocator allocator = deque.GetAllocator()!;
            NativeArray<T> array = deque.ToNativeArray();
            return new NativeQuery<T>(array.GetUnsafePointer(), deque.Length, allocator);
        }

        public static NativeQuery<T> AsQuery<T>(this NativeSet<T> set) where T : unmanaged
        {
            if (set.IsEmpty)
            {
                return default;
            }

            Allocator allocator = set.GetAllocator()!;
            NativeArray<T> array = set.ToNativeArray();
            return new NativeQuery<T>(array.GetUnsafePointer(), set.Length, allocator);
        }

        public static NativeQuery<KeyValuePair<TKey, TValue>> AsQuery<TKey, TValue>(this NativeMap<TKey, TValue> map) where TKey : unmanaged where TValue : unmanaged
        {
            if (map.IsEmpty)
            {
                return default;
            }

            Allocator allocator = map.GetAllocator()!;
            NativeArray<KeyValuePair<TKey, TValue>> array = map.ToNativeArray();
            return new NativeQuery<KeyValuePair<TKey, TValue>>(array.GetUnsafePointer(), array.Length, allocator);
        }

        public static NativeQuery<KeyValuePair<TKey, TValue>> AsQuery<TKey, TValue>(this NativeSortedMap<TKey, TValue> map) where TKey : unmanaged where TValue : unmanaged
        {
            if (map.IsEmpty)
            {
                return default;
            }

            Allocator allocator = map.GetAllocator()!;
            NativeArray<KeyValuePair<TKey, TValue>> array = map.ToNativeArray();
            return new NativeQuery<KeyValuePair<TKey, TValue>>(array.GetUnsafePointer(), array.Length, allocator);
        }
    }
}
