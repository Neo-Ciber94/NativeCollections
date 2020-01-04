using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NativeCollections.Allocators;
using NativeCollections.Managed;

namespace NativeCollections
{
    public static class NativeSliceExtensions
    {
        /// <summary>
        /// Gets a <see cref="NativeArray{T}"/> from the elements of this slice.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="slice">The slice.</param>
        /// <returns>A native array with the elements of the slice.</returns>
        unsafe public static NativeArray<T> ToNativeArray<T>(this in NativeSlice<T> slice) where T : unmanaged
        {
            return ToNativeArray(slice, Allocator.Default);
        }

        /// <summary>
        /// Gets a <see cref="NativeArray{T}"/> from the elements of this slice.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="slice">The slice.</param>
        /// <param name="allocator">The allocator to use.</param>
        /// <returns>A native array with the elements of the slice.</returns>
        unsafe public static NativeArray<T> ToNativeArray<T>(this in NativeSlice<T> slice, Allocator allocator) where T : unmanaged
        {
            if (slice.IsEmpty)
            {
                return default;
            }

            void* buffer = allocator.Allocate<T>(slice.Length);
            Unsafe.CopyBlockUnaligned(buffer, slice._pointer, (uint)(sizeof(T) * slice.Length));
            return new NativeArray<T>(buffer, slice.Length, allocator);
        }

        /// <summary>
        /// Gets a <see cref="NativeList{T}"/> from the elements of this slice.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="slice">The slice.</param>
        /// <returns>A native list with the elements of the slice.</returns>
        unsafe public static NativeList<T> ToNativeList<T>(this in NativeSlice<T> slice) where T : unmanaged
        {
            return ToNativeList(slice, Allocator.Default);
        }

        /// <summary>
        /// Gets a <see cref="NativeList{T}"/> from the elements of this slice.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="slice">The slice.</param>
        /// <param name="allocator">The allocator to use.</param>
        /// <returns>A native list with the elements of the slice.</returns>
        unsafe public static NativeList<T> ToNativeList<T>(this in NativeSlice<T> slice, Allocator allocator) where T : unmanaged
        {
            if (slice.IsEmpty)
            {
                return default;
            }

            void* buffer = allocator.Allocate<T>(slice.Length);
            Unsafe.CopyBlockUnaligned(buffer, slice._pointer, (uint)(sizeof(T) * slice.Length));
            return new NativeList<T>(buffer, slice.Length, allocator);
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> from the elements of the slice.
        /// </summary>
        /// <typeparam name="T">Type of the elemnts.</typeparam>
        /// <param name="slice">The slice.</param>
        /// <returns>An enumerable over the elements of the slice.</returns>
        unsafe public static IEnumerable<T> AsEnumerable<T>(this in NativeSlice<T> slice)
        {
            return new RefEnumerable<T>(slice._pointer, slice.Length);
        }
    }
}
