using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Provides methods for filter, reduce and convert elements of a collection.
    /// Most of the methods dispose the NativeQuery after called, each result of the method generate
    /// a new query on memory differents from <see cref="Enumerable"/> what is lazy.
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeQueryDebugView<>))]
    unsafe public ref partial struct NativeQuery<T> where T : unmanaged
    {
        internal readonly T* _buffer;
        private readonly int _length;
        private readonly int _allocatorID;

        /// <summary>
        /// Gets an empty <see cref="NativeQuery{T}" />.
        /// </summary>
        public static NativeQuery<T> Empty => new NativeQuery<T>(Allocator.Default);

        internal NativeQuery(Allocator? allocator)
        {
            if (allocator == null)
            {
                this = new NativeQuery<T>(Allocator.Default);
                return;
            }

            if (!Allocator.IsCached(allocator))
            {
                throw new ArgumentException("The allocator is not in cache");
            }

            _buffer = null;
            _length = 0;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeQuery{T}" /> struct.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="length">The length.</param>
        public NativeQuery(void* pointer, int length) : this(pointer, length, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeQuery{T}" /> struct.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="length">The length.</param>
        /// <param name="allocator">The allocator to use.</param>
        public NativeQuery(void* pointer, int length, Allocator allocator)
        {
            if (pointer == null)
            {
                throw new ArgumentException("pointer is null");
            }

            if (length < 0)
            {
                throw new ArgumentException("length cannot be negative.");
            }

            if (!Allocator.IsCached(allocator))
            {
                throw new ArgumentException("The allocator is not in cache");
            }

            _buffer = (T*)pointer;
            _length = length;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// The number of elements in this query.
        /// </summary>
        public readonly int Length => _length;

        /// <summary>
        /// Determines if this query have not elements.
        /// </summary
        public readonly bool IsEmpty => _length == 0;

        /// <summary>
        /// Determines if this query is allocated.
        /// </summary
        public readonly bool IsValid => _buffer != null;

        /// <summary>
        /// Gets a read-only reference to the element at the specified index.
        /// </summary
        /// <exception cref="ArgumentOutOfRangeException">If the index is negative or greater than the length.</exception>
        public readonly ref T this[int index]
        {
            get
            {
                if(index < 0 || index > _length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index.ToString());
                }

                return ref _buffer[index];
            }
        }

        /// <summary>
        /// Gets the allocator.
        /// </summary>
        /// <returns>The allocator used for this query.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal readonly Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        /// <summary>
        /// Perform a foreach loop over the elements of this query and then dispose this query.
        /// </summary>
        /// <param name="action">The action to execute over each element.</param>
        [DisposeAfterCall]
        public void ForEach(Action<T> action)
        {
            foreach (var e in this)
            {
                action(e);
            }
        }

        /// <summary>
        /// Gets a string representation of the elements of this query.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (_length == 0)
            {
                return "[]";
            }

            StringBuilder sb = StringBuilderCache.Acquire();
            Enumerator enumerator = GetEnumerator();
            sb.Append('[');

            if (enumerator.MoveNext())
            {
                while (true)
                {
                    sb.Append(enumerator.Current.ToString());

                    if (enumerator.MoveNext())
                    {
                        sb.Append(',');
                        sb.Append(' ');
                    }
                    else
                    {
                        break;
                    }
                }
            }

            sb.Append(']');
            return StringBuilderCache.ToStringAndRelease(ref sb!);
        }

        /// <summary>
        /// Releases the resources used for this instance.
        /// </summary>
        public void Dispose()
        {
            if (_buffer == null)
            {
                return;
            }

            Allocator? allocator = GetAllocator();

            if (allocator != null)
            {
                allocator.Free(_buffer);
                this = default;
            }
        }

        /// <summary>
        /// Gets an enumerator over the elements of this query that may be dispose the query at the end.
        /// </summary>
        /// <returns>An enumerator over the elements of the query.</returns>
        public readonly Enumerator GetEnumerator()
        {
            if (_buffer == null)
            {
                return default;
            }

            fixed (NativeQuery<T>* p = &this)
            {
                return new Enumerator(p, dispose: true);
            }
        }

        /// <summary>
        /// Gets an enumerator over the elements of this query that may be dispose the query at the end.
        /// </summary>
        /// <param name="disposing">If set to true Dispose will be called for the query after the enumeration is done.</param>
        /// <returns>An enumerator over the elements of the query.</returns>
        public readonly Enumerator GetEnumerator(bool disposing)
        {
            if(_buffer == null)
            {
                return default;
            }

            fixed(NativeQuery<T>* p = &this)
            {
                return new Enumerator(p, disposing);
            }
        }

        /// <summary>
        /// Gets an enumerator over the elemnets of a <see cref="NativeQuery{T}"/> that can dispose the query after enumeration.
        /// </summary>
        public struct Enumerator
        {
            private NativeQuery<T>* _query;
            private int _pos;
            private bool _dispose;

            internal Enumerator(NativeQuery<T>* query, bool dispose)
            {
                _query = query;
                _pos = -1;
                _dispose = dispose;
            }

            /// <summary>
            /// Gets a reference to the current value.
            /// </summary>
            public readonly ref T Current
            {
                get
                {
                    if (_query == null)
                    {
                        throw new InvalidOperationException("Enumerator is invalid");
                    }

                    if (_pos < 0 || _pos > _query->_length)
                    {
                        throw new ArgumentOutOfRangeException(_pos.ToString());
                    }

                    ref NativeQuery<T> query = ref *_query;
                    return ref query[_pos];
                }
            }

            /// <summary>
            /// Moves to the next value.
            /// </summary>
            public bool MoveNext()
            {
                if (_query != null)
                {
                    int index = _pos + 1;
                    if (index < _query->_length)
                    {
                        _pos = index;
                        return true;
                    }

                    if (_query != null && _dispose)
                    {
                        Dispose();
                    }
                }

                return false;
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _pos = -1;
            }

            /// <summary>
            /// Dispose this enumerator and the <see cref="NativeQuery{T}"/>.
            /// </summary>
            public void Dispose()
            {
                if (_query != null)
                {
                    _query->Dispose();
                    this = default;
                }
            }
        }
    }
}
