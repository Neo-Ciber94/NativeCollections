using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NativeCollections.Managed
{
    unsafe public class RefEnumerable<T> : IEnumerable<T>
    {
        private readonly void* _pointer;
        private readonly int _length;

        public RefEnumerable(void* pointer, int length)
        {
            _pointer = pointer;
            _length = length;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(_pointer, _length);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(_pointer, _length);
        }

        /// <summary>
        /// Represents an enumerator that returns values by reference.
        /// </summary>
        /// <typeparam name="T">Type of the values.</typeparam>
        unsafe public struct Enumerator : IEnumerator<T>
        {
            private readonly void* _pointer;
            private readonly int _length;
            private int _index;

            /// <summary>
            /// Initializes a new instance of the <see cref="RefEnumerator{T}" /> struct.
            /// </summary>
            /// <param name="pointer">The pointer to the elements.</param>
            /// <param name="length">The number of elements to enumerate.</param>
            internal Enumerator(void* pointer, int length)
            {
                if (pointer == null)
                {
                    throw new ArgumentException("Pointer is null", nameof(pointer));
                }

                if (length < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(length), "Length is negative");
                }

                _pointer = pointer;
                _length = length;
                _index = -1;
            }

            /// <summary>
            /// Gets a reference to the current value.
            /// </summary>
            public T Current
            {
                get
                {
                    if (_index < 0 || _index > _length)
                    {
                        throw new IndexOutOfRangeException($"{_index} > {_length}");
                    }

                    return Unsafe.Add(ref Unsafe.AsRef<T>(_pointer), _index);
                }
            }

            /// <summary>
            /// Moves to the next value.
            /// </summary>
            public bool MoveNext()
            {
                int i = _index + 1;
                if (i < _length)
                {
                    _index = i;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Invalidates this enumerator.
            /// </summary>
            public void Dispose()
            {
                if (_pointer == null)
                {
                    return;
                }

                this = default;
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _index = -1;
            }

            object? IEnumerator.Current => Current;
        }
    }
}
