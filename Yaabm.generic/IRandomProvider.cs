namespace Yaabm.generic
{
    public interface IRandomProvider
    {
        /// <summary>
        ///     Generate a random value from a uniform[0, 1] distribution
        /// </summary>
        /// <returns>A random value between 0 and 1</returns>
        double NextDouble();

        /// <summary>
        ///     Generate a random permutation of items in a list
        /// </summary>
        /// <param name="count">The number of items in the list</param>
        /// <returns>An array of items by index number in random order</returns>
        int[] Shuffle(int count);

        /// <summary>
        ///     Sample a random event
        /// </summary>
        /// <param name="probabilityOfSuccess">The probability that the event will occur</param>
        /// <returns>True if success</returns>
        bool Chance(double probabilityOfSuccess);

        int SamplePoisson(double lambda);

        int NextInt(int minValue, int maxValue);

        System.Random RandomSource { get; }

        int[] RandomSelect(int n, int k);
    }
}