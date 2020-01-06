using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NativeCollections.Utility
{
    unsafe public static partial class UnsafeUtilities
    {
        /// <summary>
        /// Counts the number of elements that match the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of elements that meet the condition.</returns>
        public static int Count<T>(void* pointer, int startElementOffset, int endElementOffset, Predicate<T> predicate)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(startElementOffset < endElementOffset, "Invalid range");

            ref T ptr = ref Unsafe.AsRef<T>(pointer);
            int count = 0;

            while(startElementOffset < endElementOffset)
            {
                ref T value = ref Unsafe.Add(ref ptr, startElementOffset);
                if (predicate(value))
                {
                    count++;
                }

                ++startElementOffset;
            }

            return count;
        }

        /// <summary>
        /// Checks that all the elements meet the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="predicate">The condition.</param>
        /// <returns><c>true</c> If all the elements meet the predicate; otherwise <c>false</c>.</returns>
        public static bool AllMatch<T>(void* pointer, int startElementOffset, int endElementOffset, Predicate<T> predicate)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(startElementOffset < endElementOffset, "Invalid range");

            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            for (int i = startElementOffset; i < endElementOffset; ++i)
            {
                ref T value = ref Unsafe.Add(ref ptr, i);
                if (!predicate(value))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks that neither the element meet the specified condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="predicate">The condition.</param>
        /// <returns><c>true</c> If none the elements meet the predicate; otherwise <c>false</c>.</returns>
        public static bool NoneMatch<T>(void* pointer, int startElementOffset, int endElementOffset, Predicate<T> predicate)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(startElementOffset < endElementOffset, "Invalid range");

            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            for (int i = startElementOffset; i < endElementOffset; ++i)
            {
                ref T value = ref Unsafe.Add(ref ptr, i);
                if (predicate(value))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Finds the first element that match the predicate.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The index of the first element that match the predicate.</returns>
        public static int FindFirst<T>(void* pointer, int startElementOffset, int endElementOffset, Predicate<T> predicate)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(startElementOffset < endElementOffset, "Invalid range");

            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            for(int i = startElementOffset; i < endElementOffset; ++i)
            {
                ref T value = ref Unsafe.Add(ref ptr, i);
                if (predicate(value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Finds the last element that match the predicate.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The index of the last element that match the predicate.</returns>
        public static int FindLast<T>(void* pointer, int startElementOffset, int endElementOffset, Predicate<T> predicate)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(startElementOffset < endElementOffset, "Invalid range");

            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            for (int i = endElementOffset - 1; i >= startElementOffset; --i)
            {
                ref T value = ref Unsafe.Add(ref ptr, i);
                if (predicate(value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Determines whether the pointer contains the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the pointer contains the value; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains<T>(void* pointer, int startElementOffset, int endElementOffset, T value)
        {
            return Contains(pointer, startElementOffset, endElementOffset, value, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Determines whether the pointer contains the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        ///   <c>true</c> if the pointer contains the value; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains<T>(void* pointer, int startElementOffset, int endElementOffset, T value, IEqualityComparer<T> comparer)
        {
            return IndexOf(pointer, startElementOffset, endElementOffset, value, comparer) >= 0;
        }

        /// <summary>
        /// Gets the index of the first element that match the given value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>The index of the first element or -1 if not found.</returns>
        public static int IndexOf<T>(void* pointer, int startElementOffset, int endElementOffset, T value)
        {
            return IndexOf(pointer, startElementOffset, endElementOffset, value, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Gets the index of the first element that match the given value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The index of the first element or -1 if not found.</returns>
        public static int IndexOf<T>(void* pointer, int startElementOffset, int endElementOffset, T value, IEqualityComparer<T> comparer)
        {
            if (startElementOffset > endElementOffset)
            {
                throw new ArgumentOutOfRangeException($"startElementOffset cannot be greater than endElementOffset: {startElementOffset} > {endElementOffset}");
            }

            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            for(int i = startElementOffset; i < endElementOffset; i++)
            {
                ref T current = ref Unsafe.Add(ref ptr, i);
                if(comparer.Equals(current, value))
                {
                    return i;
                }
            }

            return -1;
        }
        
        /// <summary>
        /// Gets the index of the last element that match the given value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>The index of the last element or -1 if not found.</returns>
        public static int LastIndexOf<T>(void* pointer, int startElementOffset, int endElementOffset, T value)
        {
            return LastIndexOf(pointer, startElementOffset, endElementOffset, value, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Gets the index of the last element that match the given value.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The index of the last element or -1 if not found.</returns>
        public static int LastIndexOf<T>(void* pointer, int startElementOffset, int endElementOffset, T value, IEqualityComparer<T> comparer)
        {
            Debug.Assert(startElementOffset < endElementOffset, "Invalid range");

            ref T ptr = ref Unsafe.AsRef<T>(pointer);

            for (int i = endElementOffset - 1; i >= startElementOffset; i--)
            {
                ref T current = ref Unsafe.Add(ref ptr, i);
                if (comparer.Equals(current, value))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Perform a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="value">The value.</param>
        /// <returns>The element <c>index</c> of the located value, or <c>~index</c> if not found.</returns>
        public static int BinarySearch<T>(void* pointer, int startElementOffset, int endElementOffset, T value)
        {
            return BinarySearch(pointer, startElementOffset, endElementOffset, value, Comparer<T>.Default);
        }

        /// <summary>
        /// Perform a binary search to locate the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="pointer">The pointer.</param>
        /// <param name="startElementOffset">The start element offset.</param>
        /// <param name="endElementOffset">The end element offset.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The element <c>index</c> of the located value, or <c>~index</c> if not found.</returns>
        public static int BinarySearch<T>(void* pointer, int startElementOffset, int endElementOffset, T value, IComparer<T> comparer)
        {
            Debug.Assert(pointer != null, "Pointer is null");
            Debug.Assert(startElementOffset < endElementOffset, "Invalid range");

            ref T startAddress = ref Unsafe.AsRef<T>(pointer);

            while (startElementOffset <= endElementOffset)
            {
                int mid = startElementOffset + (endElementOffset - startElementOffset) / 2;
                ref T current = ref Unsafe.Add<T>(ref startAddress, mid);
                int comp = comparer.Compare(value, current);

                if (comp == 0)
                {
                    return mid;
                }
                if(comp < 0)
                {
                    endElementOffset = mid - 1;
                }
                else
                {
                    startElementOffset = mid + 1;
                }
            }

            return ~startElementOffset;
        }
    }
}
