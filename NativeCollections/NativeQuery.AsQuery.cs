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

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this array.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <returns>A query over the elements.</returns>
        public static NativeQuery<T> AsQuery<T>(this NativeArray<T> array) where T : unmanaged
        {
            if (array.IsEmpty)
            {
                return default;
            }

            Allocator allocator = array.GetAllocator()!;
            void* buffer = AllocateCopy<T>(array.GetUnsafePointer(), array.Length, allocator);
            return new NativeQuery<T>(buffer, array.Length, allocator);
        }

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this list.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>A query over the elements.</returns>
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

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this stack.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="stack">The stack.</param>
        /// <returns>A query over the elements.</returns>
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

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this queue.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="queue">The queue.</param>
        /// <returns>A query over the elements.</returns>
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

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this deque.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="array">The deque.</param>
        /// <returns>A query over the elements.</returns>
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

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this set.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="set">The set.</param>
        /// <returns>A query over the elements.</returns>
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

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this slice.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="slice">The slice.</param>
        /// <returns>A query over the elements.</returns>
        public static NativeQuery<T> AsQuery<T>(this NativeSlice<T> slice) where T: unmanaged
        {
            return AsQuery(slice, Allocator.Default);
        }

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this slice.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="slice">The slice.</param>
        /// <param name="allocator">The allocator to use.</param>
        /// <returns>A query over the elements.</returns>
        public static NativeQuery<T> AsQuery<T>(this NativeSlice<T> slice, Allocator allocator) where T: unmanaged
        {
            if (slice.IsEmpty)
            {
                return default;
            }

            NativeArray<T> array = slice.ToNativeArray();
            return new NativeQuery<T>(array.GetUnsafePointer(), slice.Length, allocator);
        }

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this set.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="set">The set.</param>
        /// <param name="allocator">The allocator to use.</param>
        /// <returns>A query over the elements.</returns>
        public static NativeQuery<T> AsQuery<T>(this NativeSortedSet<T> set) where T: unmanaged
        {
            if (set.IsEmpty)
            {
                return default;
            }

            Allocator allocator = set.GetAllocator()!;
            NativeArray<T> array = set.ToNativeArray();
            return new NativeQuery<T>(array.GetUnsafePointer(), set.Length, allocator);
        }

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this map.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="map">The map.</param>
        /// <returns>A query over the elements.</returns>
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

        /// <summary>
        /// Gets a <see cref="NativeQuery{T}"/> over the elements of this map.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="map">The map.</param>
        /// <returns>A query over the elements.</returns>
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

        /// <summary>
        /// Gets a <see cref="NativeQuery{char}"/> of the <see langword="char"/> of this <see cref="NativeString"/>.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>A query over the chars of this string.</returns>
        public static NativeQuery<char> AsQuery(this NativeString str)
        {
            if (str.IsEmpty)
            {
                return NativeQuery<char>.Empty;
            }

            Allocator allocator = str.GetAllocator()!;
            char* buffer = (char*)AllocateCopy<char>(str._buffer, str.Length, allocator);
            return new NativeQuery<char>(buffer, str.Length, allocator);
        }
    }
}
