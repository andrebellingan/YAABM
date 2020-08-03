using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yaabm.generic.Random
{
    public class WeightedItem<T>
    {
        public WeightedItem(T item, double weight)
        {
            Item = item;
            Weight = weight;
        }

        public T Item { get; }

        public double Weight { get; }
    }

    public class Node<T>
    {
        public double Weight { get; set; }
        public WeightedItem<T> Value { get; set; }
        public double TotalWeight { get; set; }

        public Node(double weight, WeightedItem<T> value, double totalWeight)
        {
            Weight = weight;
            Value = value;
            TotalWeight = totalWeight;
        }
    }

    public class WeightedSampler<T>
    {
        public static List<T> PickMultipleItems(List<WeightedItem<T>> cards, int numberToDraw, System.Random random)
        {
            var pickedCards = new List<WeightedItem<T>>();

            var heap = GenerateHeap(cards);

            for (var i = 0; i < numberToDraw; i++)
            {
                pickedCards.Add(PopFromHeap(heap, random));
            }

            return pickedCards.Select(p => p.Item).ToList();
        }

        private static List<Node<T>> GenerateHeap(List<WeightedItem<T>> cards)
        {
            var nodes = new List<Node<T>>();
            nodes.Add(null);

            foreach (var card in cards)
            {
                nodes.Add(new Node<T>(card.Weight, card, card.Weight));
            }

            for (var i = nodes.Count - 1; i > 1; i--)
            {
                nodes[i >> 1].TotalWeight += nodes[i].TotalWeight;
            }

            return nodes;
        }

        private static WeightedItem<T> PopFromHeap(List<Node<T>> heap, System.Random random)
        {
            WeightedItem<T> card = null;

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
            card = heap[i].Value;

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
