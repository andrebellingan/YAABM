using System.Collections.Generic;

namespace Yaabm.generic
{
    public abstract class PopulationDynamics<TAgent> 
        where TAgent : Agent<TAgent>
    {
        protected PopulationDynamics(int capacity)
        {
            _allAgents = new HashSet<TAgent>(capacity);
            _agentsById = new Dictionary<int, TAgent>(capacity);
        }

        protected PopulationDynamics()
        {
            _allAgents = new HashSet<TAgent>();
            _agentsById = new Dictionary<int, TAgent>();
        }

        private int _nextAgentId = -1;

        protected MultiStateModel<TAgent> MultiStateModel { get; private set; }

        public void Initialize(MultiStateModel<TAgent> multiStateModel)
        {
            MultiStateModel = multiStateModel;
        }

        private readonly HashSet<TAgent> _allAgents;

        private readonly Dictionary<int, TAgent> _agentsById;

        public TAgent AgentById(int id)
        {
            return _agentsById[id];
        }

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

        public HashSet<TAgent> EnumeratePopulation()
        {
            return new HashSet<TAgent>(_allAgents);
        }

        public HashSet<int> EnumeratePopulationIds()
        {
            return new HashSet<int>(_agentsById.Keys);
        }

        internal TAgent CreateAgent()
        {
            var newAgent = GenerateNewAgent(GetNextId());
            newAgent.OnStateChange += HandleAgentStateChange;
            newAgent.OnHomeAreaChanged += HandleAgentHomeAreaChange;
            _allAgents.Add(newAgent);
            _agentsById.Add(newAgent.Id, newAgent);
            AgentAdded(newAgent);
            return newAgent;
        }

        private void HandleAgentHomeAreaChange(TAgent agent, LocalArea<TAgent> previousArea)
        {
            if (agent.HomeArea == previousArea) return;

            agent.HomeArea.ResidentIds.Add(agent.Id);

            previousArea?.ResidentIds.Remove(agent.Id);
        }

        public delegate void AgentEvent(TAgent agent);

        public AgentEvent OnAgentAdded { get; set; } 

        private void AgentAdded(TAgent newAgent)
        {
            OnAgentAdded?.Invoke(newAgent);
        }

        protected abstract TAgent GenerateNewAgent(int id);

        public Agent<TAgent>.StateChangeDelegate OnAgentStateChange { get; set; }

        private int GetNextId()
        {
            _nextAgentId++;
            return _nextAgentId;
        }

        public abstract IEnumerable<Encounter<TAgent>> GetEncounters(TAgent agent, IRandomProvider random);

        public virtual IEnumerable<TAgent> GetInfectiousAgents()
        {
            return _infectiousAgents;
        }
    }
}
