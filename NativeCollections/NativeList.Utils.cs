using System;
using System.Runtime.CompilerServices;

namespace NativeCollections
{
    public static class NativeList
    {
        unsafe public static void* GetUnsafePointer<T>(ref NativeList<T> list) where T : unmanaged
        {
            if (!list.IsValid)
                throw new ArgumentException("The list is invalid");

            return list._buffer;
        }

        unsafe public static ref T GetReference<T>(ref NativeList<T> list) where T : unmanaged
        {
            if (!list.IsValid)
                throw new ArgumentException("The list is invalid");

            return ref Unsafe.AsRef<T>(list._buffer);
        }
    }
}
