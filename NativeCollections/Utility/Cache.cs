using System;

namespace NativeCollections.Utility
{
    public sealed class Cache<T> : ICache<T, int> where T : class
    {
        private readonly T?[] _cacheInstances;
        private int _nextID = 0;

        public Cache(int cacheSize)
        {
            if (cacheSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cacheSize), cacheSize.ToString());
            }

            _cacheInstances = new T[cacheSize];
        }

        public int Add(T instance)
        {
            int index = _nextID;
            int length = _cacheInstances.Length;

            if (index != length)
            {
                _cacheInstances[index] = instance;
                int id = index + 1;
                _nextID += 1;
                return id;
            }

            return -1;
        }

        public bool Remove(int id)
        {
            int index = id - 1;
            if (index > 0 && index <= _cacheInstances.Length)
            {
                if (_cacheInstances[index] != null)
                {
                    _cacheInstances[index] = null;
                    return true;
                }
            }

            return false;
        }

        public T? GetByID(int id)
        {
            int index = id - 1;

            if(index > 0 && index < _cacheInstances.Length)
            {
                return _cacheInstances[index];
            }

            return null;
        }
    }
}
