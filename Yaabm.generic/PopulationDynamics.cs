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

        private readonly HashSet<TAgent> _infectiousAgents = new HashSet<TAgent>();

        private void HandleAgentStateChange(TAgent agent, ModelState<TAgent> previousState)
        {
            if (agent.CurrentState.IsInfectious && !_infectiousAgents.Contains(agent))
            {
                _infectiousAgents.Add(agent);
            }

            if (!agent.CurrentState.IsInfectious && _infectiousAgents.Contains(agent))
            {
                _infectiousAgents.Remove(agent);
            }

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
            newAgent.OnStateChange += HandleAgentStateChange;
            newAgent.OnHomeAreaChanged += HandleAgentHomeAreaChange;
            newAgent.SetCurrentState(initialState, day);
            _allAgents.Add(newAgent);
            AgentAdded(newAgent);
            return newAgent;
        }

        private void HandleAgentHomeAreaChange(TAgent agent, LocalArea<TAgent> previousArea)
        {
            if (agent.HomeArea == previousArea) return;

            agent.HomeArea.Population.Add(agent);

            previousArea?.Population.Remove(agent);
        }

        public delegate void AgentEvent(TAgent agent);

        public AgentEvent OnAgentAdded { get; set; } 

        private void AgentAdded(TAgent newAgent)
        {
            OnAgentAdded?.Invoke(newAgent);
        }

        public TAgent CreateAgent(int day)
        {
            return CreateAgent(MultiStateModel.DefaultState, day);
        }

        protected abstract TAgent GenerateNewAgent(int id);

        public Agent<TAgent>.StateChangeDelegate OnAgentStateChange { get; set; }

        private int GetNextId()
        {
            _nextAgentId++;
            return _nextAgentId;
        }

        public abstract IEnumerable<TAgent> GetContacts(TAgent agent, IRandomProvider random);

        public IEnumerable<TAgent> GetInfectiousAgents()
        {
            return _infectiousAgents;
        }
    }
}
