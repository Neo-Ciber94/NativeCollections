using System;

namespace NativeCollections
{
    public static class NativeMapExtensions
    {
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
        public static void DisposeMapAndKeys<TKey, TValue>(this ref NativeMap<TKey, TValue> map) where TKey: unmanaged, IDisposable where TValue: unmanaged
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
    }
}
