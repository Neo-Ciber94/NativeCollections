using System.Runtime.CompilerServices;

namespace NativeCollections
{
    unsafe public readonly ref struct ByReference<T>
    {
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
    }
}
