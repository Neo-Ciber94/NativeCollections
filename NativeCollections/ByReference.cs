using System;
using System.Runtime.CompilerServices;

namespace NativeCollections
{
    unsafe public readonly ref struct ByReference<T>
    {
        public static ByReference<T> Null => default;

        private readonly void* _pointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByReference(ref T value)
        {
            _pointer = Unsafe.AsPointer(ref value);
        }

        public ref readonly T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.AsRef<T>(_pointer);
        }

        public bool HasValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _pointer != null;
        }

        public override string ToString()
        {
            return HasValue ? Value!.ToString()! : "null";
        }

        public override int GetHashCode()
        {
            return (int)((long)_pointer & int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            throw new NotSupportedException();
        }

        public static implicit operator ByReference<T>(ByMutableReference<T> reference)
        {
            return new ByReference<T>(ref reference.Value);
        }

        public static bool operator ==(ByReference<T> left, ByReference<T> right)
        {
            return left._pointer == right._pointer;
        }

        public static bool operator !=(ByReference<T> left, ByReference<T> right)
        {
            return !(left == right);
        }
    }

    unsafe public ref struct ByMutableReference<T>
    {
        private readonly void* _pointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByMutableReference(ref T value)
        {
            _pointer = Unsafe.AsPointer(ref value);
        }

        public ref T Value
        {
            get => ref Unsafe.AsRef<T>(_pointer);
        }

        public bool HasValue
        {
            get => _pointer != null;
        }

        public override string ToString()
        {
            return HasValue ? Value!.ToString()! : "null";
        }

        public override int GetHashCode()
        {
            return (int)((long)_pointer & int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            throw new NotSupportedException();
        }

        public static bool operator ==(ByMutableReference<T> left, ByMutableReference<T> right)
        {
            return left._pointer == right._pointer;
        }

        public static bool operator !=(ByMutableReference<T> left, ByMutableReference<T> right)
        {
            return !(left == right);
        }
    }
}
