using System;
using System.Diagnostics;

namespace Samples
{
    /// <summary>
    /// Custom random number generator.
    /// </summary>
    public struct RandomNumberGenerator
    {
        private readonly int _seed;
        private int _state1;
        private int _state2;

        /// <summary>
        /// Gets a default random number generator.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public static RandomNumberGenerator Default
        {
            get
            {
                long ticks = Stopwatch.GetTimestamp();
                int seed = (int)(ticks & int.MaxValue);
                return new RandomNumberGenerator(seed);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomNumberGenerator"/> struct.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public RandomNumberGenerator(int seed)
        {
            _seed = seed;

            unchecked
            {
                _state1 = seed + int.MaxValue;
                _state2 = seed * int.MaxValue;
            }
        }

        /// <summary>
        /// Gets a random int value from int.MinValue to int.MaxValue.
        /// </summary>
        /// <returns>A random int value.</returns>
        public int NextInt()
        {
            return Sample();
        }

        /// <summary>
        /// Gets a random int value from 0 to max (inclusive).
        /// </summary>
        /// <param name="max">The maximum value (inclusive).</param>
        /// <returns>A random int value.</returns>
        public int NextInt(int max)
        {
            return NextInt(0, max);
        }

        /// <summary>
        /// Gets a random int value from min (inclusive) to max (inclusive).
        /// </summary>
        /// <param name="min">The minimum value inclusive.</param>
        /// <param name="max">The maximum value inclusive.</param>
        /// <returns>A random int value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If min is greater than max.</exception>
        public int NextInt(int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException($"min > max: {min} > {max}");
            }

            int value = Sample();

            if (value < 0)
            {
                value = -value;
            }

            max = value % (max + 1);
            return (max - min) + min;
        }

        /// <summary>
        /// Gets a random long value from long.MinValue to long.MaxValue.
        /// </summary>
        /// <returns>A random long value.</returns>
        public long NextLong()
        {
            return (long)NextInt() >> 4;
        }

        /// <summary>
        /// Gets a random double value from double.MinValue to double.MaxValue.
        /// </summary>
        /// <returns>A random double value.</returns>
        public double NextDouble()
        {
            return Sample() * (1.0 / int.MaxValue);
        }

        /// <summary>
        /// Gets a random float value from float.MinValue to float.MaxValue.
        /// </summary>
        /// <returns>A random float value.</returns>
        public float NextFloat()
        {
            return Sample() * (1f / int.MaxValue);
        }

        /// <summary>
        /// Gets a random <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        /// <returns>A random bool value.</returns>
        public bool NextBool()
        {
            return NextInt(0, 1) == 1;
        }

        /// <summary>
        /// Fills the <see cref="Span{T}"/> with random bytes.
        /// </summary>
        /// <param name="destination">The destination.</param>
        public void NextBytes(Span<byte> destination)
        {
            for(int i = 0; i < destination.Length; i++)
            {
                destination[i] = (byte)(NextInt() & byte.MaxValue);
            }
        }

        private int Sample()
        {
            int result = _seed;
            unchecked
            {
                result += _state1;
                result *= _state2;

                _state1 += 1234;
                _state2 += 5678;
            }

            return result;
        }
    }
}
