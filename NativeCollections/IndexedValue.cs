using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Represents a value with an index.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public readonly struct IndexedValue<T> : IEquatable<IndexedValue<T>>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; }
        /// <summary>
        /// Gets the index of the value.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedValue{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        public IndexedValue(T value, int index)
        {
            Value = value;
            Index = index;
        }

        /// <summary>
        /// Deconstructs this instance as a value and index.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out T value, out int index)
        {
            value = Value;
            index = Index;
        }

        /// <summary>
        /// Gets a string representation of this value.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = StringBuilderCache.Acquire();
            sb.Append('(');
            sb.Append(Index.ToString());
            sb.Append(", ");
            sb.Append(Value!.ToString());
            sb.Append(')');
            return StringBuilderCache.ToStringAndRelease(ref sb!);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return obj is IndexedValue<T> value && Equals(value);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals([AllowNull] IndexedValue<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value) &&
                   Index == other.Index;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Index);
        }

        public static bool operator ==(IndexedValue<T> left, IndexedValue<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IndexedValue<T> left, IndexedValue<T> right)
        {
            return !(left == right);
        }
    }

    public static class IndexedValue
    {
        /// <summary>
        /// Creates a new <see cref="IndexedValue{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <returns>A value with its index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IndexedValue<T> Create<T>(T value, int index)
        {
            return new IndexedValue<T>(value, index);
        }

        /// <summary>
        /// Gets an index-value representation of the elements of the enumerable.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>An indexed enumerable of the specified enumerable.</returns>
        public static IEnumerable<IndexedValue<T>> WithIndex<T>(this IEnumerable<T> enumerable)
        {
            if(enumerable is IndexedEnumerable<T>)
            {
                return (IndexedEnumerable<T>)enumerable;
            }

            return new IndexedEnumerable<T>(enumerable);
        }

        internal class IndexedEnumerable<T> : IEnumerable<IndexedValue<T>>, IEnumerator<IndexedValue<T>>
        {
            private const int Invalid = -2;
            private const int Start = -1;

            private readonly IEnumerable<T> _source;
            private IEnumerator<T>? _enumerator;
            private int _index = Start;

            public IndexedEnumerable(IEnumerable<T> enumerable)
            {
                _source = enumerable;
            }

            public IndexedValue<T> Current
            {
                get
                {
                    if(_enumerator == null || _index == Invalid)
                    {
                        throw new InvalidOperationException("Invalid state");
                    }

                    checked
                    {
                        T current = _enumerator.Current;
                        return new IndexedValue<T>(current, _index);
                    }
                }
            }

            object? IEnumerator.Current => Current;

            public void Dispose()
            {
                _index = Invalid;
            }

            public bool MoveNext()
            {
                if(_index == Invalid)
                {
                    return false;
                }

                if(_index == Start)
                {
                    _enumerator = _source.GetEnumerator();
                }

                if (_enumerator!.MoveNext())
                {
                    ++_index;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _index = Start;
            }

            public IEnumerator<IndexedValue<T>> GetEnumerator()
            {
                _enumerator = _source.GetEnumerator();
                _index = Start;
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
