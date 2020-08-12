using System;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

namespace Yaabm.generic.Random
{
    public class DefaultRandom : IRandomProvider
    {
        public DefaultRandom(int seed)
        {
            Seed = seed;
            RandomSource = new MersenneTwister(seed, true);
        }

        public int Seed { get; }

        public System.Random RandomSource { get; }

        public int SamplePoisson(double lambda)
        {
            if (lambda <= 0d) throw new ArgumentException("Lambda parameter for Poisson distribution must be > 0");

            var distribution = new Poisson(lambda, RandomSource);
            return distribution.Sample();
        }

        public int NextInt(int minValue, int maxValue)
        {
            return RandomSource.Next(minValue, maxValue);
        }

        public double NextDouble()
        {
            return RandomSource.NextDouble();
        }

        public int[] Shuffle(int n)
        {
            return Combinatorics.GeneratePermutation(n, RandomSource);
        }

        public bool Chance(double probabilityOfSuccess)
        {
            var randomValue = NextDouble();
            return randomValue <= probabilityOfSuccess;
        }

        public int NextInt()
        {
            return RandomSource.Next();
        }

        public int NextInt(int maxValue)
        {
            return RandomSource.Next(maxValue);
        }

        /// <summary>
        /// Get a sample from a ordered list of items
        /// </summary>
        /// <param name="n">Size of the population</param>
        /// <param name="k">Number of items to sample</param>
        /// <returns>An array with the index numbers of the selected items</returns>
        public int[] RandomSelect(int n, int k)
        {
            if (k > n) throw new ArgumentException("k cannot be larger than n");

            var result = new int[k];
            if (k == 0) return result;

            var arr = new bool[n];

            var itemsSelected = 0;

            while (itemsSelected < k)
            {
                var i = NextInt(n);
                if (arr[i]) continue;

                result[itemsSelected] = i;
                arr[i] = true;
                itemsSelected++;
            }

            return result;
        }

        public int SampleDaysInState(double rate)
        {
            // Need to adjust for the fact that only one transition is allowed per day
            // Then we need to convert from a continuous decrement rate to a daily one.
            var pStar = (1 - Math.Exp(-rate)) / Math.Exp(-rate);
            var p = (1 - Math.Exp(-pStar));

            // This uses the geometric distribution
            // This is the discrete equivalent of a exponential decay model
            var geometricDistribution = new Geometric(p, RandomSource);

            return geometricDistribution.Sample();
        }
    }
}