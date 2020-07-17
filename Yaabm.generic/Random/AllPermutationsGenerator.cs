using System;
using System.Collections.Generic;

namespace Yaabm.generic.Random
{
    internal class AllPermutationsGenerator
    {
        public AllPermutationsGenerator(int k)
        {
            if (k > 10) throw new ArgumentException("Cannot generate all permutations if k is greater than 11", nameof(k));

            var initialSequence = new int[k];
            for (var i = 0; i < k; i++)
            {
                initialSequence[i] = i;
            }

            Generate(k, initialSequence);
        }

        public List<int[]> AllPermutations { get; } = new List<int[]>();

        private void Generate(int k, int[] a)
        {
            if (k == 1)
            {
                AddToOutput(a);
            }
            else
            {
                Generate(k - 1, a);

                for (var i = 0; i < k - 1; i++)
                {
                    if (k % 2 == 0)
                    {
                        Swap(ref a, i, k - 1);
                    }
                    else
                    {
                        Swap(ref a, 0, k - 1);
                    }

                    Generate(k - 1, a);
                }
            }
        }

        private static void Swap(ref int[] array, int a, int b)
        {
            var tmp = array[a];
            array[a] = array[b];
            array[b] = tmp;
        }

        private void AddToOutput(int[] a)
        {
            var copyOfSequence = new int[a.Length];
            for (var i = 0; i < a.Length; i++)
            {
                copyOfSequence[i] = a[i];
            }

            AllPermutations.Add(copyOfSequence);
        }
    }
}
