using System;
using System.Collections.Generic;


namespace Yaabm.generic.Random
{
    public class Node<T>
    {
        public double Weight { get; set; }
        public WeightedChoice<T> Value { get; set; }
        public double TotalWeight { get; set; }

        public Node(double weight, WeightedChoice<T> value, double totalWeight)
        {
            Weight = weight;
            Value = value;
            TotalWeight = totalWeight;
        }
    }

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

            var heap = GenerateHeap(cards);

            for (var i = 0; i < numberToDraw; i++)
            {
                var poppedItem = PopFromHeap(heap, random);
                pickedCards.Add(poppedItem.Choice);
            }

            return pickedCards;
        }

        private static Node<T>[] GenerateHeap(List<WeightedChoice<T>> cards)
        {
            var nodes = new Node<T>[cards.Count + 1];
            nodes[0] = null;

            var k = 1;
            foreach (var card in cards)
            {
                nodes[k] = new Node<T>(card.Weight, card, card.Weight);
                k++;
            }

            for (var i = nodes.Length - 1; i > 1; i--)
            {
                nodes[i >> 1].TotalWeight += nodes[i].TotalWeight;
            }

            return nodes;
        }

        private static WeightedChoice<T> PopFromHeap(IReadOnlyList<Node<T>> heap, IRandomProvider random)
        {
            var gas = random.NextDouble() * heap[1].TotalWeight;
            var i = 1;

            while (gas >= heap[i].Weight)
            {
                gas -= heap[i].Weight;
                i <<= 1;

                if (gas >= heap[i].TotalWeight)
                {
                    gas -= heap[i].TotalWeight;
                    i += 1;
                }
            }

            var weight = heap[i].Weight;
            var card = heap[i].Value;

            heap[i].Weight = 0;

            while (i > 0)
            {
                heap[i].TotalWeight -= weight;
                i >>= 1;
            }

            return card;
        }

    }
}
