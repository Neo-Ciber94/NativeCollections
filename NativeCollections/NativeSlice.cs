using System;
using System.Runtime.CompilerServices;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public readonly ref struct NativeSlice<T>
    {
        private readonly void* _pointer;
        private readonly int _length;

        public NativeSlice(void* pointer, int length)
        {
            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            if (length < 0)
                throw new ArgumentException($"Invalid length: {length}");

            _pointer = pointer;
            _length = length;
        }

        public NativeSlice(ref T pointer, int length)
        {
            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            if (length < 0)
                throw new ArgumentException($"Invalid length: {length}");

            _pointer = Unsafe.AsPointer(ref pointer);
            _length = length;
        }

        public int Length => _length;

        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index > _length)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T pointer = ref Unsafe.AsRef<T>(_pointer);
                return ref Unsafe.Add(ref pointer, index);
            }
        }

        public ref T this[Index index]
        {
            get
            {
                int i = index.IsFromEnd ? _length - index.Value - 1 : index.Value;

                if (i < 0 || i > _length)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T pointer = ref Unsafe.AsRef<T>(_pointer);
                return ref Unsafe.Add(ref pointer, i);
            }
        }

        public NativeSlice<T> this[Range range]
        {
            get
            {
                Index startIndex = range.Start;
                Index endIndex = range.End;

                int start = startIndex.IsFromEnd ? _length - startIndex.Value : startIndex.Value;
                int end = endIndex.IsFromEnd ? _length - endIndex.Value : endIndex.Value;

                if (start < 0 || end < 0 || start > _length || end > _length || start > end)
                    throw new ArgumentOutOfRangeException("range", range.ToString());

                ref T pointer = ref Unsafe.Add(ref Unsafe.AsRef<T>(_pointer), start);
                int length = end - start + 1;
                return new NativeSlice<T>(ref pointer, length);
            }
        }

        public Span<T> Span
        {
            get => new Span<T>(_pointer, _length);
        }

        /// <summary>
        /// Gets an enumerator for iterate over the elements of this slice.
        /// </summary>
        /// <returns>An enumerator over this array elements.</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Exposes methods for iterate over the contents of a <see cref="NativeSlice{T}"/>.
        /// </summary>
        /// <seealso cref="NativeCollections.INativeContainer{T}" />
        /// <seealso cref="System.IDisposable" />
        public ref struct Enumerator
        {
            private void* _pointer;
            private int _length;
            private int _index;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="slice">The slice.</param>
            public Enumerator(in NativeSlice<T> slice)
            {
                _pointer = slice._pointer;
                _length = slice._length;
                _index = -1;
            }

            /// <summary>
            /// Gets a reference to the current value.
            /// </summary>
            /// <value>
            /// The current value.
            /// </value>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            public ref T Current
            {
                get
                {
                    if (_index < 0 || _index > _length)
                        throw new ArgumentOutOfRangeException("index", _index.ToString());

                    ref T pointer = ref Unsafe.AsRef<T>(_pointer);
                    return ref Unsafe.Add(ref pointer, _index);
                }
            }

            /// <summary>
            /// Disposes this enumerator.
            /// </summary>
            public void Dispose()
            {
                if (_pointer == null)
                    return;

                _pointer = null;
                _length = 0;
                _index = 0;
            }

            /// <summary>
            /// Moves to the next value.
            /// </summary>
            /// <returns><c>true</c> if has a next value, otherwise <c>false</c></returns>
            public bool MoveNext()
            {
                if (_pointer == null)
                    return false;

                int i = _index + 1;
                if (i < _length)
                {
                    _index = i;
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _index = -1;
            }
        }
    }
}
