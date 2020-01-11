using System;
using System.Collections.Generic;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public ref partial struct NativeQuery<T>
    {
        /// <summary>
        /// Determines if all the elements in this query match the given predicate and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns><true></true> if all the elements match the condition; otherwise false.</returns>
        [DisposeAfterCall]
        public bool All(Predicate<T> predicate)
        {
            try
            {
                foreach (ref var e in this)
                {
                    if (!predicate(e))
                    {
                        return false;
                    }
                }
            }
            finally
            {
                Dispose();
            }

            return true;
        }

        /// <summary>
        /// Determines if none the elements in this query match the given predicate and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns><true></true> if none the elements match the condition; otherwise false.</returns>
        [DisposeAfterCall]
        public bool None(Predicate<T> predicate)
        {
            try
            {
                foreach (ref var e in this)
                {
                    if (predicate(e))
                    {
                        return false;
                    }
                }
            }
            finally
            {
                Dispose();
            }

            return true;
        }

        /// <summary>
        /// Determines if any the elements in this query match the given predicate and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns><true></true> if any the elements match the condition; otherwise false.</returns>
        [DisposeAfterCall]
        public bool Any(Predicate<T> predicate)
        {
            try
            {
                foreach (ref var e in this)
                {
                    if (predicate(e))
                    {
                        return true;
                    }
                }
            }
            finally
            {
                Dispose();
            }

            return false;
        }

        /// <summary>
        /// Determines whether this instance contains the value and then dispose this instance.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>
        ///   <c>true</c> if the query contains the value; otherwise, <c>false</c>.
        /// </returns>
        [DisposeAfterCall]
        public bool Contains(T value)
        {
            bool result = UnsafeUtilities.Contains(_buffer, 0, _length, value);
            Dispose();
            return result;
        }

        /// <summary>
        /// Counts the number of elements that match the predicate and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of elements that match the predicate.</returns>
        [DisposeAfterCall]
        public int Count(Predicate<T> predicate)
        {
            int count = 0;

            try
            {
                foreach (ref var e in this)
                {
                    if (predicate(e))
                    {
                        ++count;
                    }
                }
            }
            finally
            {
                Dispose();
            }

            return count;
        }

        /// <summary>
        /// Gets the first element in the query and then dispose this instance.
        /// </summary>
        /// <returns>The first element in the query.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty.</exception>
        [DisposeAfterCall]
        public T First()
        {
            if (_length == 0)
            {
                Dispose();
                throw new InvalidOperationException("NativeQuery is empty");
            }

            ref T first = ref _buffer[0];
            Dispose();
            return first;
        }

        /// <summary>
        /// Gets the first element that match the predicate and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The first element that match the condition.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty or not value is found.</exception>
        [DisposeAfterCall]
        public T First(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                Dispose();
                throw new InvalidOperationException("NativeQuery is empty");
            }

            try
            {
                int index = UnsafeUtilities.FindFirst(_buffer, 0, _length, predicate);

                if (index >= 0)
                {
                    ref T value = ref _buffer[index];
                    return value;
                }
                else
                {
                    Dispose();
                    throw new InvalidOperationException("Cannot any value that meet the condition.");
                }
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Gets the first value or the default if the query is empty and then dispose this instance.
        /// </summary>
        /// <returns>The first element in the query or the default value of T if not found.</returns>
        [DisposeAfterCall]
        public T FirstOrDefault()
        {
            if (_length == 0)
            {
                return default;
            }

            ref T first = ref _buffer[0];
            Dispose();
            return first;
        }

        /// <summary>
        /// Gets the first element that match the condition or the default value and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The first element that match the condition or the default if not found.</returns>
        [DisposeAfterCall]
        public T FirstOrDefault(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                return default;
            }

            int index = UnsafeUtilities.FindFirst(_buffer, 0, _length, predicate);

            try
            {
                if (index >= 0)
                {
                    ref T value = ref _buffer[index];
                    return value;
                }
                else
                {
                    return default;
                }
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Gets the last element in the query and then dispose this instance.
        /// </summary>
        /// <returns>The last element in the query.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty.</exception>
        [DisposeAfterCall]
        public T Last()
        {
            if (_length == 0)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            ref T last = ref _buffer[_length - 1];
            Dispose();
            return last;
        }

        /// <summary>
        /// Gets the last element that match the predicate and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The last element that match the condition.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty or not value is found.</exception>
        [DisposeAfterCall]
        public T Last(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                Dispose();
                throw new InvalidOperationException("NativeQuery is empty");
            }

            try
            {
                int index = UnsafeUtilities.FindLast(_buffer, 0, _length, predicate);

                if (index >= 0)
                {
                    ref T value = ref _buffer[index];
                    return value;
                }
                else
                {
                    Dispose();
                    throw new InvalidOperationException("No value match the condition.");
                }
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Gets the last value or the default if the query is empty and then dispose this instance.
        /// </summary>
        /// <returns>The last element in the query or the default value of T if not found.</returns>
        [DisposeAfterCall]
        public T LastOrDefault()
        {
            if (_length == 0)
            {
                return default;
            }

            ref T last = ref _buffer[_length - 1];
            Dispose();
            return last;
        }

        /// <summary>
        /// Gets the last element that match the condition or the default value and then dispose this instance.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The last element that match the condition or the default if not found.</returns>
        [DisposeAfterCall]
        public T LastOrDefault(Predicate<T> predicate)
        {
            if (_length == 0)
            {
                return default;
            }

            try
            {
                int index = UnsafeUtilities.FindLast(_buffer, 0, _length, predicate);

                if (index >= 0)
                {
                    ref T value = ref _buffer[index];
                    return value;
                }
                else
                {
                    return default;
                }
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Performs a reducion operation over the elements in the query using the specified <see cref="Func{T1, T2, TResult}"/> and then dispose this instance.
        /// </summary>
        /// <param name="func">The reduction function, where the first value is the total and the second the current.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="InvalidOperationException">If the query is empty.</exception>
        [DisposeAfterCall]
        public T Reduce(Func<T, T, T> func)
        {
            if (_length == 0)
            {
                throw new InvalidOperationException("NativeQuery is empty");
            }

            T? result = null;

            try
            {
                foreach (ref var e in this)
                {
                    if (result == null)
                    {
                        result = e;
                    }
                    else
                    {
                        result = func(result.Value, e);
                    }
                }
            }
            finally
            {
                Dispose();
            }

            return result!.Value;
        }

        /// <summary>
        /// Performs a reducion operation over the elements in the query using the specified <see cref="Func{T1, T2, TResult}"/> and the seed value and then dispose this instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="seed">The initial value.</param>
        /// <param name="func">The reduction function.</param>
        /// <returns>The result of the operation.</returns>
        [DisposeAfterCall]
        public TResult Reduce<TResult>(TResult seed, Func<TResult, T, TResult> func) where TResult : unmanaged
        {
            if (_length == 0)
            {
                return seed;
            }

            TResult? result = seed;

            try
            {
                foreach (ref var e in this)
                {
                    result = func(result.Value, e);
                }
            }
            finally
            {
                Dispose();
            }

            return result!.Value;
        }
    }
}
