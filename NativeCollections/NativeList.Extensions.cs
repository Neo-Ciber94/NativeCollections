using System;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public static class NativeListExtensions
    {
        public static void Sort<T>(this NativeList<T> list) where T : unmanaged, IComparable<T>
        {
            if (!list.IsValid)
                return;

            void* pointer = list._buffer;
            NativeCollectionUtility.QuickSort<T>(pointer, 0, list.Length - 1);
        }

        public static void Sort<T>(this NativeList<T> list, int start, int end) where T: unmanaged, IComparable<T>
        {
            if (!list.IsValid)
                return;

            void* pointer = list._buffer;
            NativeCollectionUtility.QuickSort<T>(pointer, start, end);
        }

        public static int BinarySearch<T>(this NativeList<T> list, T value) where T : unmanaged, IComparable<T>
        {
            if (!list.IsValid)
                return -1;

            return NativeCollectionUtility.BinarySearch(list._buffer, 0, list.Length - 1, value);
        }

        public static int BinarySearch<T>(this NativeList<T> list, int start, T value) where T : unmanaged, IComparable<T>
        {
            if (!list.IsValid)
                return -1;

            return NativeCollectionUtility.BinarySearch(list._buffer, start, list.Length - 1, value);
        }

        public static int BinarySearch<T>(this NativeList<T> list, int start, int end, T value) where T: unmanaged, IComparable<T>
        {
            if (!list.IsValid)
                return -1;

            return NativeCollectionUtility.BinarySearch(list._buffer, start, end, value);
        }
    }
}
