using System;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// Static wrapper class for a random number generation object.
    /// </summary>
    public static class RNG
    {
        private static readonly Random RandomNumberGenerator = new Random();

        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        /// <returns></returns>
        public static int Next()
        {
            return RandomNumberGenerator.Next();
        }

        /// <summary>
        /// Returns a random integer within the specified bounds.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max">Exclusive upper bound. This means any returned value must be lower than max.</param>
        /// <returns></returns>
        public static int Next(int min, int max)
        {
            return RandomNumberGenerator.Next(min, max);
        }

        /// <summary>
        /// Returns a random float within the specified bounds.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float NextFloat(float min, float max)
        {
            return (float) (RandomNumberGenerator.NextDouble() * (max - min)) + min;
        }

        /// <summary>
        /// Returns a random double between 0.0 and 1.0
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double NextDouble()
        {
            return RandomNumberGenerator.NextDouble();
        }

        /// <summary>
        /// Returns a random boolean.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool NextBoolean()
        {
            return RandomNumberGenerator.Next() % 2 == 0;
        }
    }
}