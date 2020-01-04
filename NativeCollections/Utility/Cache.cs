using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Utility
{
    public interface IWithID
    {
        public int ID { get; }
    }

    public sealed class Cache<T> where T : class, IWithID
    {
        private readonly T[] _cacheInstance;
        private int _nextID = 0;

        public Cache(int capacity)
        {
            _cacheInstance = new T[capacity];
        }

        public int AddInstance(T instance)
        {
            return -1;
        }

        public bool RemoveInstance(int id)
        {
            return false;
        }

        public bool RemoveInstance(T instance)
        {
            return RemoveInstance(instance.ID);
        }
    }
}
