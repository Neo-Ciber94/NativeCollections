using System;
using System.Collections.Generic;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public static class NativeArrayExtensions
    {
        public static void Sort<T>(this NativeArray<T> array) where T : unmanaged, IComparable<T>
        {
            if (!array.IsValid)
                return;

            void* pointer = array._buffer;
            NativeCollectionUtils.QuickSort<T>(pointer, 0, array.Length - 1);
        }

        public static void Sort<T>(this NativeArray<T> array, int start, int end) where T: unmanaged, IComparable<T>
        {
            if (!array.IsValid)
                return;

            void* pointer = array._buffer;
            NativeCollectionUtils.QuickSort<T>(pointer, start, end);
        }

        public static int BinarySearch<T>(this NativeArray<T> array, T value) where T : unmanaged, IComparable<T>
        {
            if (!array.IsValid)
                return -1;

            return NativeCollectionUtils.BinarySearch(array._buffer, 0, array.Length - 1, value);
        }

        public static int BinarySearch<T>(this NativeArray<T> array, int start, T value) where T : unmanaged, IComparable<T>
        {
            if (!array.IsValid)
                return -1;

            return NativeCollectionUtils.BinarySearch(array._buffer, start, array.Length - 1, value);
        }

        public static int BinarySearch<T>(this NativeArray<T> array, int start, int end, T value) where T: unmanaged, IComparable<T>
        {
            if (!array.IsValid)
                return -1;

            return NativeCollectionUtils.BinarySearch(array._buffer, start, end, value);
        }
    }
}
