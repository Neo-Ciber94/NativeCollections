using System;
using System.Runtime.CompilerServices;

namespace NativeCollections
{
    /// <summary>
    /// Represents a reference to a value.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    unsafe public readonly ref struct ByReference<T>
    {
        private readonly void* _pointer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByReference{T}" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ByReference(ref T value)
        {
            _pointer = Unsafe.AsPointer(ref value);
        }
        
        internal ByReference(void* pointer)
        {
            _pointer = pointer;
        }

        /// <summary>
        /// Gets a reference to the value.
        /// </summary>
        public ref T Value
        {
           
            get
            {
                if(_pointer == null)
                {
                    throw new InvalidOperationException("No value");
                }

                return ref Unsafe.AsRef<T>(_pointer);
            }
        }
        
        /// <summary>
        /// Determines if there is a reference to a value.
        /// </summary>
        public bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _pointer != null;
        }

        /// <summary>
        /// Gets a string representation of the value of this reference.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return IsNull ? Value!.ToString()! : "null";
        }

        public override int GetHashCode()
        {
            return (int)((long)_pointer & int.MaxValue);
        }

        public override bool Equals(object? obj)
        {
            throw new NotSupportedException();
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
}
