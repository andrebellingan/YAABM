using System;
using System.Collections.Generic;
using System.Linq;

namespace Yaabm.generic
{
    public abstract class PopulationDynamics<T> where T : Agent<T>
    {
        private int _nextAgentId = -1;

        private readonly Dictionary<ModelState<T>, Dictionary<int, T>> _agentsByState = new Dictionary<ModelState<T>, Dictionary<int, T>>();

        private bool _hasBeenInitialized;

        protected MultiStateModel<T> MultiStateModel { get; private set; }

        public void Initialize(MultiStateModel<T> multiStateModel)
        {
            MultiStateModel = multiStateModel;

            foreach (var state in multiStateModel.States)
            {
                _agentsByState.Add(state, new Dictionary<int, T>());
            }

            _hasBeenInitialized = true;
        }

        private readonly Dictionary<int, T> _allAgents = new Dictionary<int, T>();

        internal void ProcessAgentMovingState(T agent, ModelState<T> destinationState)
        {
            if (!_hasBeenInitialized) throw new InvalidOperationException("Population dynamics has not been initialized. Call Initialize");

            foreach (var state in MultiStateModel.States)
            {
                if (_agentsByState[state].ContainsKey(agent.Id)) _agentsByState[state].Remove(agent.Id);
            }

            _agentsByState[destinationState].Add(agent.Id, agent);
        }

        public T[] EnumeratePopulation(IRandomProvider randomProvider, bool shuffled = true)
        {
            if (!_hasBeenInitialized) throw new InvalidOperationException("Population dynamics has not been initialized. Call Initialize");

            if (!shuffled)
            {
                return _allAgents.Values.ToArray();
            }

            var randomOrder = randomProvider.Shuffle(_allAgents.Count);

            var shuffledPopulation = new T[_allAgents.Count];
            var allPersonIds = _allAgents.Keys.ToArray();

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < _allAgents.Count; index++)
            {
                var personToChoose = randomOrder[index];
                var personId = allPersonIds[personToChoose];
                shuffledPopulation[index] = _allAgents[personId];
            }

            return shuffledPopulation;
        }

        internal void AddAgent(T agent)
        {
            if (!_hasBeenInitialized) throw new InvalidOperationException("Population dynamics has not been initialized. Call Initialize");
            agent.SetId(GetNextId());
            _allAgents.Add(agent.Id, agent);
        }

        private int GetNextId()
        {
            _nextAgentId++;
            return _nextAgentId;
        }

        public abstract IEnumerable<T> GetContacts(T agent, IRandomProvider random);

        public abstract IEnumerable<T> GetInfectiousAgents();
    }
}
