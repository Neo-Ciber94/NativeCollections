using System;

namespace NativeCollections.Utility
{
    /// <summary>
    /// Represents a cache.
    /// </summary>
    /// <typeparam name="TObject">The type of the instance to cache.</typeparam>
    /// <typeparam name="TID">The type of the id.</typeparam>
    public interface ICache<TObject, TID> where TObject: class where TID : IComparable<TID>
    {
        /// <summary>
        /// Adds a instance to the cache.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The id of the instance in the cache.</returns>
        public TID Add(TObject instance);
        /// <summary>
        /// Removes the instance with the given id from the cache.
        /// </summary>
        /// <param name="id">The id of the instance to remove.</param>
        /// <returns><c>true</c> if the instance was removed, otherwise <c>false</c>.</returns>
        public bool Remove(TID id);
        /// <summary>
        /// Gets an instance from the cache by an id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The instance with the given id.</returns>
        public TObject? GetByID(TID id);
    }
}
