using System;
using System.Runtime.CompilerServices;

namespace NativeCollections.Utility
{
    /// <summary>
    /// Extension methods for any object.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Perform an operation over this object.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="action">The action.</param>
        /// <returns>The same object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Also<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }
    }
}
