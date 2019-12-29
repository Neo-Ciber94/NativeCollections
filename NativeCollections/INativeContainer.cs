using System;

namespace NativeCollections
{
    /// <summary>
    /// Represents a container for unmanaged data.
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    public interface INativeContainer<T>
    {
        /// <summary>
        /// Gets the number of elements in this container.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length { get; }

        /// <summary>
        /// Checks if this container is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is allocated; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; }

        /// <summary>
        /// Gets a value indicating whether this container has no elements.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this container is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty { get; }

        /// <summary>
        /// Copies the content of this container to a <see cref="Span{T}"/>
        /// </summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="destinationIndex">Start index of the destination where start to copy.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyTo(in Span<T> span, int destinationIndex, int count);
    }
}
