using System.Collections.Generic;


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
        public static List<T> PickMultipleItems(List<WeightedItem<T>> cards, int numberToDraw, IRandomProvider random)
        {
            var pickedCards = new List<T>(numberToDraw);

            var heap = GenerateHeap(cards);

            for (var i = 0; i < numberToDraw; i++)
            {
                var poppedItem = PopFromHeap(heap, random);
                pickedCards.Add(poppedItem.Item);
            }

            return pickedCards;
        }

        private static Node<T>[] GenerateHeap(List<WeightedItem<T>> cards)
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

        private static WeightedItem<T> PopFromHeap(IReadOnlyList<Node<T>> heap, IRandomProvider random)
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
