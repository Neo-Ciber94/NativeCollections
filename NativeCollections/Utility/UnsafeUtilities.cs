using System.Runtime.CompilerServices;

namespace NativeCollections.Utility
{
    /// <summary>
    /// A set of utility methods for unsafe operations.
    /// </summary>
    unsafe public static partial class UnsafeUtilities
    {
        /// <summary>
        /// Gets the address of the given value.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A pointer to the specified value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* AddressOf<T>(ref T value) where T: unmanaged
        {
            return (T*)Unsafe.AsPointer(ref value);
        }

        /// <summary>
        /// Gets a nulls the reference.
        /// </summary>
        /// <typeparam name="T">Type of the reference.</typeparam>
        /// <returns>An unsafe null reference.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T NullRef<T>()
        {
            return ref Unsafe.AsRef<T>(null);
        }

        /// <summary>
        /// Determines whether the given object reference is null.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if is a null reference; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullRef<T>(ref T value)
        {
            return Unsafe.AsPointer(ref value) == null;
        }
    }
}
