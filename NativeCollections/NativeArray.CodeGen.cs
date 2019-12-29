
using NativeCollections;

public static partial class NativeArray
{
    unsafe public static NativeArray<T> Create<T>(T arg1) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(1);
        array[0] = arg1;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(2);
        array[0] = arg1;
        array[1] = arg2;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(3);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(4);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(5);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;
        array[4] = arg5;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(6);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;
        array[4] = arg5;
        array[5] = arg6;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(7);
        array[0] = arg1;
        array[1] = arg2;
        array[2] = arg3;
        array[3] = arg4;
        array[4] = arg5;
        array[5] = arg6;
        array[6] = arg7;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(8);
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

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(9);
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

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(10);
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

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10, T arg11) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(11);
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
        array[10] = arg11;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10, T arg11, T arg12) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(12);
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
        array[10] = arg11;
        array[11] = arg12;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10, T arg11, T arg12, T arg13) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(13);
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
        array[10] = arg11;
        array[11] = arg12;
        array[12] = arg13;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10, T arg11, T arg12, T arg13, T arg14) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(14);
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
        array[10] = arg11;
        array[11] = arg12;
        array[12] = arg13;
        array[13] = arg14;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10, T arg11, T arg12, T arg13, T arg14, T arg15) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(15);
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
        array[10] = arg11;
        array[11] = arg12;
        array[12] = arg13;
        array[13] = arg14;
        array[14] = arg15;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10, T arg11, T arg12, T arg13, T arg14, T arg15, T arg16) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(16);
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
        array[10] = arg11;
        array[11] = arg12;
        array[12] = arg13;
        array[13] = arg14;
        array[14] = arg15;
        array[15] = arg16;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10, T arg11, T arg12, T arg13, T arg14, T arg15, T arg16, T arg17) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(17);
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
        array[10] = arg11;
        array[11] = arg12;
        array[12] = arg13;
        array[13] = arg14;
        array[14] = arg15;
        array[15] = arg16;
        array[16] = arg17;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10, T arg11, T arg12, T arg13, T arg14, T arg15, T arg16, T arg17, T arg18) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(18);
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
        array[10] = arg11;
        array[11] = arg12;
        array[12] = arg13;
        array[13] = arg14;
        array[14] = arg15;
        array[15] = arg16;
        array[16] = arg17;
        array[17] = arg18;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10, T arg11, T arg12, T arg13, T arg14, T arg15, T arg16, T arg17, T arg18, T arg19) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(19);
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
        array[10] = arg11;
        array[11] = arg12;
        array[12] = arg13;
        array[13] = arg14;
        array[14] = arg15;
        array[15] = arg16;
        array[16] = arg17;
        array[17] = arg18;
        array[18] = arg19;

        return array;
    }

    unsafe public static NativeArray<T> Create<T>(T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9, T arg10, T arg11, T arg12, T arg13, T arg14, T arg15, T arg16, T arg17, T arg18, T arg19, T arg20) where T : unmanaged
    {
        NativeArray<T> array = new NativeArray<T>(20);
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
        array[10] = arg11;
        array[11] = arg12;
        array[12] = arg13;
        array[13] = arg14;
        array[14] = arg15;
        array[15] = arg16;
        array[16] = arg17;
        array[17] = arg18;
        array[18] = arg19;
        array[19] = arg20;

        return array;
    }
}