using System;
using System.Diagnostics;

namespace Samples
{
    [Flags]
    public enum RandomCharKind
    {
        Upper = 1,
        Lower = 2,
        Digit = 4,
        Symbol = 8
    }


    /// <summary>
    /// Custom random number generator.
    /// </summary>
    public struct RandomNumberGenerator
    {
        private int _state;

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
            unchecked
            {
                _state = seed + int.MaxValue;
                NextState();
            }
        }

        /// <summary>
        /// Gets a random int value from int.MinValue to int.MaxValue.
        /// </summary>
        /// <returns>A random int value.</returns>
        public int NextInt()
        {
            return NextState();
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

            if(min == max)
            {
                return min;
            }

            return (int)(NextDouble() * ((max - min) + 1)) + min;
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
            int sample = NextState();

            if(sample < 0)
            {
                sample = -sample;
            }

            return sample * (1.0 / int.MaxValue);
        }

        /// <summary>
        /// Gets a random float value from float.MinValue to float.MaxValue.
        /// </summary>
        /// <returns>A random float value.</returns>
        public float NextFloat()
        {
            return NextState() * (1f / int.MaxValue);
        }

        /// <summary>
        /// Gets a random <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        /// <returns>A random bool value.</returns>
        public bool NextBool()
        {
            return NextInt(0, 1) == 1;
        }

        public char NextChar(RandomCharKind randomChar)
        {

            return default;
        }

        /// <summary>
        /// Fills the <see cref="Span{T}"/> with random bytes.
        /// </summary>
        /// <param name="destination">The destination.</param>
        public void NextBytes(Span<byte> destination)
        {
            for(int i = 0; i < destination.Length; ++i)
            {
                destination[i] = (byte)(NextInt() & byte.MaxValue);
            }
        }

        private int NextState()
        {
            unchecked
            {
                _state += 23457013;
                _state ^= _state << 13;
                _state ^= _state >> 17;
                _state ^= _state << 5;
                return _state;
            }
        }
    }
}
