using System;

namespace NativeCollections.Utility
{
    public sealed class SyncCache<T> : ICache<T, int> where T: class
    {
        private readonly object _obj = new object();
        private readonly T?[] _cacheInstances;
        private int _nextID = 0;

        public SyncCache(int cacheSize)
        {
            if (cacheSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cacheSize), cacheSize.ToString());
            }

            _cacheInstances = new T[cacheSize];
        }

        public int Add(T instance)
        {
            int id = -1;
            int length = _cacheInstances.Length;

            lock (_obj)
            {
                int index = _nextID;

                if (index != length)
                {
                    _cacheInstances[index] = instance;
                    id = index + 1;
                    _nextID += 1;
                    return id;
                }
            }

            return id;
        }

        public bool Remove(int id)
        {
            int index = id - 1;
            lock (_obj)
            {
                if (index > 0 && index <= _cacheInstances.Length)
                {
                    if (_cacheInstances[index] != null)
                    {
                        _cacheInstances[index] = null;
                        return true;
                    }
                }
            }

            return false;
        }

        public T? GetByID(int id)
        {
            int index = id - 1;
            T? instance = null;

            lock (_obj)
            {
                if (index > 0 && index < _cacheInstances.Length)
                {
                    instance = _cacheInstances[index];
                }
            }

            return instance;
        }
    }
}
