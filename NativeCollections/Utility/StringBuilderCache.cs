using System;
using System.Text;

namespace NativeCollections.Utility
{
    /// <summary>
    /// Provides a cache instance of <see cref="StringBuilder"/> to reduces allocations.
    /// </summary>
    public static class StringBuilderCache
    {
        private const int DefaultCapacity = 64;
        private const int MaxCapacity = 360;

        [ThreadStatic]
        private static StringBuilder? _cachedInstance;

        /// <summary>
        /// Gets the cache <see cref="StringBuilder"/> instance.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        /// <returns>A StringBuilder instance.</returns>
        public static StringBuilder Acquire(int capacity = DefaultCapacity)
        {
            if (capacity <= MaxCapacity)
            {
                StringBuilder stringBuilder = _cachedInstance!;
                if (stringBuilder != null && capacity <= stringBuilder.Capacity)
                {
                    _cachedInstance = null;
                    stringBuilder.Clear();
                    return stringBuilder;
                }
            }

            return new StringBuilder(capacity);
        }

        /// <summary>
        /// Returns the specified instance to the cache.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        public static void Release(ref StringBuilder? stringBuilder)
        {
            if (stringBuilder != null && stringBuilder.Capacity <= MaxCapacity)
            {
                _cachedInstance = stringBuilder;
            }

            stringBuilder = null;
        }

        /// <summary>
        /// Gets the string created by the builder and then return it to the cache.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <returns>The string created by the builder.</returns>
        public static string ToStringAndRelease(ref StringBuilder? stringBuilder)
        {
            string result = stringBuilder!.ToString();
            Release(ref stringBuilder);
            return result;
        }
    }
}
