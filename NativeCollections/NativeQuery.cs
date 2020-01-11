using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Provides methods for filter, reduce and convert elements of a collection.
    /// Most of the methods dispose the NativeQuery after called.
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeQueryDebugView<>))]
    unsafe public ref partial struct NativeQuery<T> where T : unmanaged
    {
        internal readonly T* _buffer;
        private readonly int _length;
        private readonly int _allocatorID;

        internal NativeQuery(Allocator? allocator)
        {
            if(allocator == null)
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

        internal NativeQuery(void* pointer, int length, Allocator allocator)
        {
            if(pointer == null)
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
        public readonly void ForEach(Action<T> action)
        {
            try
            {
                foreach (var e in this)
                {
                    action(e);
                }
            }
            finally
            {
                Dispose();
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
            RefEnumerator<T> enumerable = GetEnumerator();
            sb.Append('[');

            if (enumerable.MoveNext())
            {
                while (true)
                {
                    sb.Append(enumerable.Current.ToString());

                    if (enumerable.MoveNext())
                    {
                        sb.Append(", ");
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
        /// Gets an enumerator over the elements of this query.
        /// </summary>
        /// <returns>An enumerator over the elements of the query.</returns>
        public readonly RefEnumerator<T> GetEnumerator()
        {
            if (_buffer == null)
            {
                return default;
            }

            return new RefEnumerator<T>(_buffer, _length);
        }
    }
}
