using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NativeCollections.Utility
{
    /// <summary>
    /// Provides a set of preconditions.
    /// </summary>
    public static class Requires
    {
        /// <summary>
        /// Checks the given argument is not null.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="obj">The argument.</param>
        /// <param name="msg">The error message.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException">If the obj is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>([AllowNull] T obj, string? msg = null, string ? paramName = null)
        {
            if(obj == null)
            {
                throw new ArgumentNullException(msg, paramName);
            }
        }

        /// <summary>
        /// Checks the specified argument condition is true.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="msg">The error message.</param>
        /// <param name="paramName">The name of the parameter evaluated.</param>
        /// <exception cref="ArgumentException">If the condition fail.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Argument(bool condition, string? msg = null, string ? paramName = null)
        {
            if (!condition)
            {
                throw new ArgumentException(msg, paramName);
            }
        }

        /// <summary>
        /// Checks the specified range condition is true.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="msg">The error message.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the condition fail.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Range(bool condition, string? msg = null)
        {
            if (!condition)
            {
                throw new ArgumentOutOfRangeException(msg);
            }
        }
    }
}
