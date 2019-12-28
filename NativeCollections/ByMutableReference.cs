using System;
using System.Runtime.CompilerServices;

namespace NativeCollections
{
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
