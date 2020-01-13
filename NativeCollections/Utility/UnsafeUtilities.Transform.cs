using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace NativeCollections.Utility
{
    unsafe public static partial class UnsafeUtilities
    {
        /// <summary>
        /// Replaces all the elements that meet the condition with the <c>newValue</c>.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset of objects of type T.</param>
        /// <param name="endElementOffset">The end element offset of objects of type T.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="predicate">The condition to use.</param>
        /// <returns>The number of elements replaced.</returns>
        public static int ReplaceIf<T>(void* pointer, int startElementOffset, int endElementOffset, T newValue, Predicate<T> predicate)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(startElementOffset < endElementOffset, "Invalid range");

            ref T startAddress = ref Unsafe.AsRef<T>(pointer);
            int count = 0;

            for (int i = startElementOffset; i < endElementOffset; ++i)
            {
                ref T current = ref Unsafe.Add(ref startAddress, i);
                if (predicate(current))
                {
                    current = newValue;
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Replaces all the elements equals than the specified <c>value</c> with the <c>newValue</c>.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset of objects of type T.</param>
        /// <param name="endElementOffset">The end element offset of objects of type T.</param>
        /// <param name="value">The value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The number of elements replaced.</returns>
        public static int ReplaceAll<T>(void* pointer, int startElementOffset, int endElementOffset, T value, T newValue)
        {
            return ReplaceAll(pointer, startElementOffset, endElementOffset, value, newValue, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Replaces all the elements equals than the specified <c>value</c> with the <c>newValue</c>.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset of objects of type T.</param>
        /// <param name="endElementOffset">The end element offset of objects of type T.</param>
        /// <param name="value">The value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The number of elements replaced</returns>
        public static int ReplaceAll<T>(void* pointer, int startElementOffset, int endElementOffset, T value, T newValue, IEqualityComparer<T> comparer)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(startElementOffset < endElementOffset, "Invalid range");

            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            int count = 0;
            for (int i = startElementOffset; i < endElementOffset; ++i)
            {
                ref T current = ref Unsafe.Add(ref startAddress, i);
                if (comparer.Equals(current, value))
                {
                    current = newValue;
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Reverses the order of the elements of the specified pointer.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset of objects of type T.</param>
        /// <param name="endElementOffset">The end element offset of objects of type T.</param>
        public static void Reverse<T>(void* pointer, int startElementOffset, int endElementOffset)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(startElementOffset <= endElementOffset, "Invalid range");

            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            while (startElementOffset <= endElementOffset)
            {
                ref T left = ref Unsafe.Add(ref startAddress, startElementOffset);
                ref T right = ref Unsafe.Add(ref startAddress, endElementOffset);

                Swap(ref left, ref right);
                ++startElementOffset;
                --endElementOffset;
            }
        }

        /// <summary>
        /// Swaps the values at the specified references.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">The first value.</param>
        /// <param name="right">The left value.</param>
        public static void Swap<T>(ref T left, ref T right)
        {
            T temp = left;
            left = right;
            right = temp;
        }
    }
}
