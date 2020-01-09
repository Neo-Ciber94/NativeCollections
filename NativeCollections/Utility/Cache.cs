using System;

namespace NativeCollections.Utility
{
    public class Cache<T> where T : class
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

        public virtual int Add(T instance)
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

        public virtual bool Remove(int id)
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

        public virtual T? GetByID(int id)
        {
            int index = id - 1;

            if(index > 0 && index < _cacheInstances.Length)
            {
                return _cacheInstances[index];
            }

            return null;
        }
    }

    public class SyncCache<T> : Cache<T> where T: class
    {
        private readonly object _obj = new object();
        public SyncCache(int cacheSize) : base(cacheSize) { }

        public override int Add(T instance)
        {
            lock (_obj)
            {
                return base.Add(instance);
            }
        }

        public override bool Remove(int id)
        {
            lock (_obj)
            {
                return base.Remove(id);
            }
        }

        public override T? GetByID(int id)
        {
            lock (_obj)
            {
                return base.GetByID(id);
            }
        }
    }
}
