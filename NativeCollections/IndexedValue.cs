using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Represents a value with an index.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public readonly struct IndexedValue<T>
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
    }

    public static class IndexedValueExtensions
    {
        public static IEnumerable<IndexedValue<T>> WithIndex<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select((e, index) => new IndexedValue<T>(e, index));
        }
    }
}
