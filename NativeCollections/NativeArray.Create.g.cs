using System.Runtime.CompilerServices;
using NativeCollections;
using NativeCollections.Allocators;

public static partial class NativeArray
{
    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 1 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <returns>A new NativeArray of 1 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe public static NativeArray<T> Create<T>(T arg1) where T : unmanaged
    {
        return Create(Allocator.Default, arg1);
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 1 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="allocator">The allocator to use.</param>
    /// <returns>A new NativeArray of 1 elements.</returns>
    unsafe public static NativeArray<T> Create<T>(Allocator allocator, T arg1) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(1, allocator);
        array[0] = arg1;
        return array;
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 2 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <returns>A new NativeArray of 2 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2) where T : unmanaged
    {
        return Create(Allocator.Default, arg1, arg2);
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 2 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="allocator">The allocator to use.</param>
    /// <returns>A new NativeArray of 2 elements.</returns>
    unsafe public static NativeArray<T> Create<T>(Allocator allocator, T arg1, T arg2) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(2, allocator);
        array[0] = arg1;
        array[1] = arg2;
        return array;
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 3 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <returns>A new NativeArray of 3 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3) where T : unmanaged
    {
        return Create(Allocator.Default, arg1, arg2, arg3);
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 3 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="allocator">The allocator to use.</param>
    /// <returns>A new NativeArray of 3 elements.</returns>
    unsafe public static NativeArray<T> Create<T>(Allocator allocator, T arg1, T arg2, T arg3) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(3, allocator);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        return array;
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 4 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <returns>A new NativeArray of 4 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4) where T : unmanaged
    {
        return Create(Allocator.Default, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 4 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="allocator">The allocator to use.</param>
    /// <returns>A new NativeArray of 4 elements.</returns>
    unsafe public static NativeArray<T> Create<T>(Allocator allocator, T arg1, T arg2, T arg3, T arg4) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(4, allocator);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;
        return array;
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 5 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <returns>A new NativeArray of 5 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5) where T : unmanaged
    {
        return Create(Allocator.Default, arg1, arg2, arg3, arg4, arg5);
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 5 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="allocator">The allocator to use.</param>
    /// <returns>A new NativeArray of 5 elements.</returns>
    unsafe public static NativeArray<T> Create<T>(Allocator allocator, T arg1, T arg2, T arg3, T arg4, T arg5) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(5, allocator);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;
        array[4] = arg5;
        return array;
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 6 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="arg6">The argument 6.</param>
    /// <returns>A new NativeArray of 6 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6) where T : unmanaged
    {
        return Create(Allocator.Default, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 6 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="arg6">The argument 6.</param>
    /// <param name="allocator">The allocator to use.</param>
    /// <returns>A new NativeArray of 6 elements.</returns>
    unsafe public static NativeArray<T> Create<T>(Allocator allocator, T arg1, T arg2, T arg3, T arg4, T arg5, T arg6) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(6, allocator);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;
        array[4] = arg5;
        array[5] = arg6;
        return array;
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 7 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="arg6">The argument 6.</param>
    /// <param name="arg7">The argument 7.</param>
    /// <returns>A new NativeArray of 7 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7) where T : unmanaged
    {
        return Create(Allocator.Default, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 7 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="arg6">The argument 6.</param>
    /// <param name="arg7">The argument 7.</param>
    /// <param name="allocator">The allocator to use.</param>
    /// <returns>A new NativeArray of 7 elements.</returns>
    unsafe public static NativeArray<T> Create<T>(Allocator allocator, T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(7, allocator);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;
        array[4] = arg5;
        array[5] = arg6;
        array[6] = arg7;
        return array;
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 8 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="arg6">The argument 6.</param>
    /// <param name="arg7">The argument 7.</param>
    /// <param name="arg8">The argument 8.</param>
    /// <returns>A new NativeArray of 8 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8) where T : unmanaged
    {
        return Create(Allocator.Default, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 8 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="arg6">The argument 6.</param>
    /// <param name="arg7">The argument 7.</param>
    /// <param name="arg8">The argument 8.</param>
    /// <param name="allocator">The allocator to use.</param>
    /// <returns>A new NativeArray of 8 elements.</returns>
    unsafe public static NativeArray<T> Create<T>(Allocator allocator, T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(8, allocator);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;
        array[4] = arg5;
        array[5] = arg6;
        array[6] = arg7;
        array[7] = arg8;
        return array;
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 9 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="arg6">The argument 6.</param>
    /// <param name="arg7">The argument 7.</param>
    /// <param name="arg8">The argument 8.</param>
    /// <param name="arg9">The argument 9.</param>
    /// <returns>A new NativeArray of 9 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9) where T : unmanaged
    {
        return Create(Allocator.Default, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 9 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="arg6">The argument 6.</param>
    /// <param name="arg7">The argument 7.</param>
    /// <param name="arg8">The argument 8.</param>
    /// <param name="arg9">The argument 9.</param>
    /// <param name="allocator">The allocator to use.</param>
    /// <returns>A new NativeArray of 9 elements.</returns>
    unsafe public static NativeArray<T> Create<T>(Allocator allocator, T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(9, allocator);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;
        array[4] = arg5;
        array[5] = arg6;
        array[6] = arg7;
        array[7] = arg8;
        array[8] = arg9;
        return array;
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 10 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="arg6">The argument 6.</param>
    /// <param name="arg7">The argument 7.</param>
    /// <param name="arg8">The argument 8.</param>
    /// <param name="arg9">The argument 9.</param>
    /// <param name="arg10">The argument 10.</param>
    /// <returns>A new NativeArray of 10 elements.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10) where T : unmanaged
    {
        return Create(Allocator.Default, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }

    /// <summary>
    /// Creates a new <see cref="NativeArray{T}"/> of 10 elements.
    /// </summary>
    /// <param name="arg1">The argument 1.</param>
    /// <param name="arg2">The argument 2.</param>
    /// <param name="arg3">The argument 3.</param>
    /// <param name="arg4">The argument 4.</param>
    /// <param name="arg5">The argument 5.</param>
    /// <param name="arg6">The argument 6.</param>
    /// <param name="arg7">The argument 7.</param>
    /// <param name="arg8">The argument 8.</param>
    /// <param name="arg9">The argument 9.</param>
    /// <param name="arg10">The argument 10.</param>
    /// <param name="allocator">The allocator to use.</param>
    /// <returns>A new NativeArray of 10 elements.</returns>
    unsafe public static NativeArray<T> Create<T>(Allocator allocator, T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(10, allocator);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;
        array[4] = arg5;
        array[5] = arg6;
        array[6] = arg7;
        array[7] = arg8;
        array[8] = arg9;
        array[9] = arg10;
        return array;
    }
}