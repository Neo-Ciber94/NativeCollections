using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections
{
    public static partial class NativeContainerExtensions
    {
        /// <summary>
        /// Releases the resources used for this array and each of its elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="disposing">if set to <c>true</c> will call dispose for each element in the array.</param>
        public static void Dispose<T>(this ref NativeArray<T> array, bool disposing) where T : unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (ref var e in array)
                    {
                        e.Dispose();
                    }
                }
            }
            finally
            {
                array.Dispose();
            }
        }

        /// <summary>
        /// Releases the resources used for this list and each of its elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="disposing">if set to <c>true</c> will call dispose for each element in the list.</param>
        public static void Dispose<T>(this ref NativeList<T> list, bool disposing) where T : unmanaged, IDisposable
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
        /// Releases the resources used for this set and each of its elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="set">The set.</param>
        /// <param name="disposing">if set to <c>true</c> will call dispose for each element in the set.</param>
        public static void Dispose<T>(this ref NativeSet<T> set, bool disposing) where T: unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (ref var e in set)
                    {
                        e.Dispose();
                    }
                }
            }
            finally
            {
                set.Dispose();
            }
        }

        /// <summary>
        /// Releases the resources used for this stack and each of its elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="stack">The stack.</param>
        /// <param name="disposing">if set to <c>true</c> will call dispose for each element in the stack.</param>
        public static void Dispose<T>(this ref NativeStack<T> stack, bool disposing) where T : unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (ref var e in stack)
                    {
                        e.Dispose();
                    }
                }
            }
            finally
            {
                stack.Dispose();
            }
        }

        /// <summary>
        /// Releases the resources used for this queue and each of its elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="queue">The queue.</param>
        /// <param name="disposing">if set to <c>true</c> will call dispose for each element in the queue.</param>
        public static void Dispose<T>(this ref NativeQueue<T> queue, bool disposing) where T : unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (ref var e in queue)
                    {
                        e.Dispose();
                    }
                }
            }
            finally
            {
                queue.Dispose();
            }
        }

        /// <summary>
        /// Releases the resources used for this deque and each of its elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="deque">The deque.</param>
        /// <param name="disposing">if set to <c>true</c> will call dispose for each element in the deque.</param>
        public static void Dispose<T>(this ref NativeDeque<T> deque, bool disposing) where T : unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (ref var e in deque)
                    {
                        e.Dispose();
                    }
                }
            }
            finally
            {
                deque.Dispose();
            }
        }

        /// <summary>
        /// Releases all the resources used for this map and dispose all the keys and values.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="map">The map.</param>
        /// <param name="disposing">if <c>true</c> disposes all the keys and values.</param>
        public static void Dispose<TKey, TValue>(this ref NativeMap<TKey, TValue> map, bool disposing) where TKey : unmanaged, IDisposable where TValue : unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (ref var entry in map)
                    {
                        entry.Key.Dispose();
                        entry.Value.Dispose();
                    }
                }
            }
            finally
            {
                map.Dispose();
            }
        }

        /// <summary>
        /// Releases all the resources used for this map and dispose all the keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="map">The map.</param>
        public static void DisposeMapAndKeys<TKey, TValue>(this ref NativeMap<TKey, TValue> map) where TKey : unmanaged, IDisposable where TValue : unmanaged
        {
            try
            {
                foreach (ref var entry in map)
                {
                    entry.Key.Dispose();
                }
            }
            finally
            {
                map.Dispose();
            }
        }

        /// <summary>
        /// Releases all the resources used for this map and dispose all the values.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="map">The map.</param>
        public static void DisposeMapAndValues<TKey, TValue>(this ref NativeMap<TKey, TValue> map) where TKey : unmanaged where TValue : unmanaged, IDisposable
        {
            try
            {
                foreach (ref var entry in map)
                {
                    entry.Value.Dispose();
                }
            }
            finally
            {
                map.Dispose();
            }
        }

        /// <summary>
        /// Releases all the resources used for this map and dispose all the keys and values.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="map">The map.</param>
        /// <param name="disposing">if <c>true</c> disposes all the keys and values.</param>
        public static void Dispose<TKey, TValue>(this ref NativeSortedMap<TKey, TValue> map, bool disposing) where TKey : unmanaged, IDisposable where TValue : unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (ref var entry in map)
                    {
                        entry.Key.Dispose();
                        entry.Value.Dispose();
                    }
                }
            }
            finally
            {
                map.Dispose();
            }
        }

        /// <summary>
        /// Releases all the resources used for this map and dispose all the keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="map">The map.</param>
        public static void DisposeMapAndKeys<TKey, TValue>(this ref NativeSortedMap<TKey, TValue> map) where TKey : unmanaged, IDisposable where TValue : unmanaged
        {
            try
            {
                foreach (ref var entry in map)
                {
                    entry.Key.Dispose();
                }
            }
            finally
            {
                map.Dispose();
            }
        }

        /// <summary>
        /// Releases all the resources used for this map and dispose all the values.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="map">The map.</param>
        public static void DisposeMapAndValues<TKey, TValue>(this ref NativeSortedMap<TKey, TValue> map) where TKey : unmanaged where TValue : unmanaged, IDisposable
        {
            try
            {
                foreach (ref var entry in map)
                {
                    entry.Value.Dispose();
                }
            }
            finally
            {
                map.Dispose();
            }
        }

        /// <summary>
        /// Releases all the resources used for this multi-map and dispose all the keys and values.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="multiValueMap">The map.</param>
        /// <param name="disposing">if <c>true</c> disposes all the keys and values.</param>
        public static void Dispose<TKey, TValue>(this ref MultiValueNativeMap<TKey, TValue> multiValueMap, bool disposing) where TKey : unmanaged, IDisposable where TValue : unmanaged, IDisposable
        {
            try
            {
                if (disposing)
                {
                    foreach (var entry in multiValueMap)
                    {
                        entry.Key.Dispose();

                        foreach(ref var value in entry.Values)
                        {
                            value.Dispose();
                        }
                    }
                }
            }
            finally
            {
                multiValueMap.Dispose();
            }
        }

        /// <summary>
        /// Releases all the resources used for this multi-map and dispose all the keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="multiValueMap">The map.</param>
        public static void DisposeMapAndKeys<TKey, TValue>(this ref MultiValueNativeMap<TKey, TValue> multiValueMap) where TKey : unmanaged, IDisposable where TValue : unmanaged
        {
            try
            {
                foreach (var entry in multiValueMap)
                {
                    entry.Key.Dispose();
                }
            }
            finally
            {
                multiValueMap.Dispose();
            }
        }

        /// <summary>
        /// Releases all the resources used for this multi-map and dispose all the values.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        /// <param name="multiValueMap">The map.</param>
        public static void DisposeMapAndValues<TKey, TValue>(this ref MultiValueNativeMap<TKey, TValue> multiValueMap) where TKey : unmanaged where TValue : unmanaged, IDisposable
        {
            try
            {
                foreach (var entry in multiValueMap)
                {
                    foreach(ref var value in entry.Values)
                    {
                        value.Dispose();
                    }
                }
            }
            finally
            {
                multiValueMap.Dispose();
            }
        }
    }
}
