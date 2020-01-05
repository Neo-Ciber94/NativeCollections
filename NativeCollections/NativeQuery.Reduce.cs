using System;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public ref partial struct NativeQuery<T>
    {
        public bool All(Predicate<T> predicate)
        {
            foreach (ref var e in this)
            {
                if (!predicate(e))
                {
                    return false;
                }
            }

            Dispose();
            return true;
        }

        public bool None(Predicate<T> predicate)
        {
            foreach (ref var e in this)
            {
                if (predicate(e))
                {
                    return false;
                }
            }

            Dispose();
            return true;
        }

        public bool Any(Predicate<T> predicate)
        {
            foreach (ref var e in this)
            {
                if (predicate(e))
                {
                    return true;
                }
            }

            Dispose();
            return false;
        }

        public bool Contains(T value)
        {
            bool result = UnsafeUtilities.Contains(_buffer, 0, _length, value);
            Dispose();
            return result;
        }

        public int Count(Predicate<T> predicate)
        {
            int count = 0;
            foreach (ref var e in this)
            {
                if (predicate(e))
                {
                    ++count;
                }
            }

            Dispose();
            return count;
        }

        public T First()
        {
            if (_length == 0)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            return _buffer[0];
        }

        public T First(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            int index = UnsafeUtilities.FindFirst(_buffer, 0, _length, predicate);

            if (index >= 0)
            {
                T value = _buffer[index];
                Dispose();
                return value;
            }
            else
            {
                throw new InvalidOperationException("Cannot any value that meet the condition.");
            }
        }

        public T FirstOrDefault()
        {
            if (_length == 0)
            {
                return default;
            }

            return _buffer[0];
        }

        public T FirstOrDefault(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                return default;
            }

            int index = UnsafeUtilities.FindFirst(_buffer, 0, _length, predicate);

            if (index >= 0)
            {
                T value = _buffer[index];
                Dispose();
                return value;
            }
            else
            {
                return default;
            }
        }

        public T Last()
        {
            if (_length == 0)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            return _buffer[_length - 1];
        }

        public T Last(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            int index = UnsafeUtilities.FindLast(_buffer, 0, _length, predicate);

            if (index >= 0)
            {
                T value = _buffer[index];
                Dispose();
                return value;
            }
            else
            {
                throw new InvalidOperationException("Cannot any value that meet the condition.");
            }
        }

        public T LastOrDefault()
        {
            if (_length == 0)
            {
                return default;
            }

            return _buffer[_length - 1];
        }

        public T LastOrDefault(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                return default;
            }

            int index = UnsafeUtilities.FindLast(_buffer, 0, _length, predicate);

            if (index >= 0)
            {
                T value = _buffer[index];
                Dispose();
                return value;
            }
            else
            {
                return default;
            }
        }

        public T Reduce(Func<T, T, T> func)
        {
            if(_length == 0)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            T? result = null;

            foreach(ref var e in this)
            {
                if(result == null)
                {
                    result = e;
                }
                else
                {
                    result = func(result.Value, e);
                }
            }

            return result!.Value;
        }

        public TResult Reduce<TResult>(TResult seed, Func<TResult, T, TResult> func) where TResult: unmanaged
        {
            if (_length == 0)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            TResult? result = seed;

            foreach (ref var e in this)
            {
                result = func(result.Value, e);
            }

            return result!.Value;
        }
    }
}
