using System;
using System.Collections.Generic;
using System.Linq;

namespace Yaabm.generic
{
    public abstract class PopulationDynamics<TAgent> 
        where TAgent : Agent<TAgent>
    {
        private int _nextAgentId = -1;

        private bool _hasBeenInitialized;

        protected MultiStateModel<TAgent> MultiStateModel { get; private set; }

        public void Initialize(MultiStateModel<TAgent> multiStateModel)
        {
            MultiStateModel = multiStateModel;

            _hasBeenInitialized = true;
        }

        private readonly HashSet<TAgent> _allAgents = new HashSet<TAgent>();

        private void HandleAgentStateChange(Agent<TAgent> agent, ModelState<TAgent> previousState)
        {
            OnAgentStateChange?.Invoke(agent, previousState);
        }

        public TAgent[] EnumeratePopulation(IRandomProvider randomProvider, bool shuffled = true)
        {
            if (!_hasBeenInitialized) throw new InvalidOperationException("Population dynamics has not been initialized. Call Initialize");


            if (!shuffled)
            {
                return _allAgents.ToArray();
            }

            var randomOrder = randomProvider.Shuffle(_allAgents.Count);
            var agentsArray = _allAgents.ToArray();
            var shuffledPopulation = new TAgent[_allAgents.Count];

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < _allAgents.Count; index++)
            {
                var personToChoose = randomOrder[index];
                shuffledPopulation[index] = agentsArray[personToChoose];
            }

            return shuffledPopulation;
        }

        public TAgent CreateAgent(ModelState<TAgent> initialState, int day)
        {
            var newAgent = GenerateNewAgent(GetNextId());
            newAgent.SetCurrentState(initialState, day);
            newAgent.OnStateChange += HandleAgentStateChange;
            _allAgents.Add(newAgent);
            return newAgent;
        }

        protected abstract TAgent GenerateNewAgent(int id);

        public Agent<TAgent>.StateChangeDelegate OnAgentStateChange { get; set; }

        private int GetNextId()
        {
            _nextAgentId++;
            return _nextAgentId;
        }

        public abstract IEnumerable<TAgent> GetContacts(TAgent agent, IRandomProvider random);

        public abstract IEnumerable<TAgent> GetInfectiousAgents();
    }
}
