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
                var i = NextInt(n - 1);
                if (arr[i]) continue;

                result[itemsSelected] = i;
                arr[i] = true;
                itemsSelected++;
            }

            return result;
        }
    }
}