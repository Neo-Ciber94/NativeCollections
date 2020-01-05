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
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeQueryDebugView<>))]
    unsafe public ref partial struct NativeQuery<T> where T : unmanaged
    {
        internal readonly T* _buffer;
        private readonly int _length;
        private readonly int _allocatorID;
        
        public int Length => _length;

        public bool IsEmpty => _length == 0;

        public bool IsValid => _buffer != null;

        internal NativeQuery(void* pointer, int length, Allocator allocator)
        {
            Debug.Assert(pointer != null);
            Debug.Assert(length > 0);
            Debug.Assert(Allocator.IsCached(allocator));

            _buffer = (T*)pointer;
            _length = length;
            _allocatorID = allocator.ID;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        public void ForEach(Action<T> action)
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

        public void Dispose()
        {
            if(_buffer == null)
            {
                return;
            }

            Allocator? allocator = GetAllocator();

            if(allocator != null)
            {
                allocator.Free(_buffer);
                this = default;
            }
        }

        public RefEnumerator<T> GetEnumerator()
        {
            if(_buffer == null)
            {
                return default;
            }

            return new RefEnumerator<T>(_buffer, _length);
        }
    }
}
