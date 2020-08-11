using System;
using System.Collections.Generic;
using Yaabm.generic.Random;

namespace Yaabm.generic
{
    /// <summary>
    ///     Base definition of a disease state
    /// </summary>
    public class ModelState<T> where T : Agent<T>
    {
        private int _permutationCacheSize;

        public delegate void StateEnteredDelegate(T agent, IRandomProvider random);

        /// <summary>
        ///     What is the name of this state
        /// </summary>
        public string Name { get; set; }

        public StateEnteredDelegate OnStateEntered { get; set; }

        internal List<Transition<T>> TransitionsFromThisState { get; } = new List<Transition<T>>();

        internal List<WithinAgentTransition<T>> WithinHostTransitionsFromState { get; } = new List<WithinAgentTransition<T>>();

        public void StateEntered(T agent, IRandomProvider random)
        {
            OnStateEntered?.Invoke(agent, random); // Perform whatever initialization action is required
        }

        public override string ToString()
        {
            return Name;
        }

        public WithinAgentTransition<T>[] GetOutgoingWithinHostTransitions(bool shuffle, IRandomProvider random)
        {
            if (_permutationCacheSize != WithinHostTransitionsFromState.Count) GeneratePermutationCache();

            if (!shuffle || WithinHostTransitionsFromState.Count <= 1)
            {
                return _cachedPermutations[0]; // the first permutation is always in order
            }
            else
            {
                var permutationIdx = random.NextInt(0, _permutationCacheSize - 1);

                return _cachedPermutations[permutationIdx];
            }
        }

        private readonly List<WithinAgentTransition<T>[]> _cachedPermutations = new List<WithinAgentTransition<T>[]>();

        public ModelState(string name, bool isInfectiousState, bool isSusceptible)
        {
            Name = name;

            if (isInfectiousState && IsSusceptible) throw new ArgumentException("A state cannot be an infectious state and a susceptible one at the same time");
            IsInfectious = isInfectiousState;
            IsSusceptible = isSusceptible;

            _cachedPermutations.Add(new WithinAgentTransition<T>[0]);
        }

        public bool IsInfectious { get; }

        public bool IsSusceptible { get; }

        private void GeneratePermutationCache()
        {
            _cachedPermutations.Clear();
            var k = WithinHostTransitionsFromState.Count;

            var permutationMaker = new AllPermutationsGenerator(k);

            foreach (var permutation in permutationMaker.AllPermutations)
            {
                var transitionPermutation = new WithinAgentTransition<T>[k];

                for (var x = 0; x < k; x++)
                {
                    var idx = permutation[x];
                    transitionPermutation[x] = WithinHostTransitionsFromState[idx];
                }
                _cachedPermutations.Add(transitionPermutation);
            }

            _permutationCacheSize = WithinHostTransitionsFromState.Count;
        }
    }
}