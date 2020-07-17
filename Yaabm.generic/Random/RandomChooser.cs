using System;
using System.Collections.Generic;

namespace Yaabm.generic.Random
{
    public class RandomChooser<T> 
    {
        private const double Sensitivity = 0.0000001d;

        public static T RandomChoice(List<WeightedChoice<T>> weights, IRandomProvider randomProvider)
        {
            var totalWeight = WeightsAddUpToOne(weights);
            if (Math.Abs(1d - totalWeight) > Sensitivity ) throw new ArgumentException($"The weights for the items to choose do not add up to one. Total weight = {totalWeight}");

            var randomValue = randomProvider.NextDouble();
            for (var i = 0; i < weights.Count; i++)
            {
                var currentWeight = weights[i];

                if (i == weights.Count - 1) return currentWeight.Choice; // last item on the list so it must be chosen

                randomValue -= currentWeight.Weight;
                if (randomValue < 0) return currentWeight.Choice;
            }

            throw new Exception("Failed to make a random choice");
        }

        private static double WeightsAddUpToOne(IEnumerable<WeightedChoice<T>> weights)
        {
            var totalProbability = 0d;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var weightedChoice in weights) totalProbability += weightedChoice.Weight;

            return totalProbability;
        }
    }
}
