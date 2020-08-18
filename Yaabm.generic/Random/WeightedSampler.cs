using System;
using System.Collections.Generic;


namespace Yaabm.generic.Random
{


    public class WeightedSampler<T>
    {
        private const double Sensitivity = 0.0000001d;

        public static T PickSingleItem(List<WeightedChoice<T>> weights, IRandomProvider randomProvider)
        {
            var totalWeight = WeightsAddUpToOne(weights);
            if (Math.Abs(1d - totalWeight) > Sensitivity) throw new ArgumentException($"The weights for the items to choose do not add up to one. Total weight = {totalWeight}");

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

        public static List<T> PickMultipleItems(List<WeightedChoice<T>> cards, int numberToDraw, IRandomProvider random)
        {
            var pickedCards = new List<T>(numberToDraw);

            GenerateHeap(cards, out var weights, out var totalWeights, out var choices);

            for (var i = 0; i < numberToDraw; i++)
            {
                var poppedItem = PopFromHeap(choices, ref weights, ref totalWeights, random);
                pickedCards.Add(poppedItem);
            }

            return pickedCards;
        }

        public static List<T> PickMultipleItems(T[] inputChoices, double[] inputWeights, int numberToDraw, IRandomProvider random)
        {
            if (inputChoices.Length != inputWeights.Length) throw new ArgumentException("Choice and weight vectors must have the same length");

            var result = new List<T>(numberToDraw);

            GenerateHeap(inputChoices, inputWeights, out var weights, out var totalWeights, out var choices);

            for (var i = 0; i < numberToDraw; i++)
            {
                var poppedItem = PopFromHeap(choices, ref weights, ref totalWeights, random);
                result.Add(poppedItem);
            }

            return result;
        }

        private static void GenerateHeap(T[] inputChoices, double[] inputWeights, out double[] weights, out double[] totalWeights, out T[] choices)
        {
            var arraySize = inputChoices.Length + 1;

            weights = new double[arraySize];
            weights[0] = 0d;

            totalWeights = new double[arraySize];
            totalWeights[0] = 0d;

            choices = new T[arraySize];
            choices[0] = default(T);

            for (var k = 1; k <= inputChoices.Length; k++)
            {
                var weight = inputWeights[k - 1];
                weights[k] = weight;
                totalWeights[k] = weight;
                choices[k] = inputChoices[k - 1];

            }

            for (var i = weights.Length - 1; i > 1; i--)
            {
                var idx = i >> 1;
                totalWeights[idx] += totalWeights[i];
            }
        }

        private static void GenerateHeap(List<WeightedChoice<T>> cards, out double[] weights, out double[] totalWeights, out T[] choices)
        {
            weights = new double[cards.Count + 1];
            weights[0] = 0d;

            totalWeights = new double[cards.Count + 1];
            totalWeights[0] = 0d;

            choices = new T[cards.Count + 1];
            choices[0] = default(T);

            var k = 1;
            foreach (var card in cards)
            {
                weights[k] = card.Weight;
                totalWeights[k] = card.Weight;
                choices[k] = card.Choice;
                k++;
            }

            for (var i = weights.Length - 1; i > 1; i--)
            {
                var idx = i >> 1;
                totalWeights[idx] += totalWeights[i];
            }
        }

        private static T PopFromHeap(IReadOnlyList<T> choices, ref double[] weights, ref double[] totalWeights, IRandomProvider random)
        {
            var gas = random.NextDouble() * totalWeights[1];
            var i = 1;

            while (gas >= weights[i])
            {
                gas -= weights[i];
                i <<= 1;

                if (gas >= totalWeights[i])
                {
                    gas -= totalWeights[i];
                    i += 1;
                }
            }

            var weight = weights[i];
            var chosen = choices[i];

            weights[i] = 0;

            while (i > 0)
            {
                totalWeights[i] -= weight;
                i >>= 1;
            }

            return chosen;
        }

    }
}
