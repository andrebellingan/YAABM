using System.Collections.Generic;
using Yaabm.generic;

namespace TestSirModel.Model
{
    public class SirEnvironment : PopulationDynamics<SirAgent>
    {
        protected override SirAgent GenerateNewAgent(int id)
        {
            return new SirAgent(id);
        }

        private readonly HashSet<Encounter<SirAgent>> _allContacts = new HashSet<Encounter<SirAgent>>();

        private bool _allEncountersInitialized;

        public override IEnumerable<Encounter<SirAgent>> GetEncounters(SirAgent agent, IRandomProvider random)
        {
            if (!_allEncountersInitialized)
            {
                InitAllEncounters();
            }

            return _allContacts;
        }

        private void InitAllEncounters()
        {
            _allContacts.Clear();
            foreach (var agent in EnumeratePopulation())
            {
                _allContacts.Add(new Encounter<SirAgent>() {Agent = agent, EncounterInformation = null});
            }

            _allEncountersInitialized = true;
        }

        public override IEnumerable<SirAgent> GetInfectiousAgents()
        {
            var dummyAgent = new SirAgent(int.MaxValue);
            var stateModel = (SirStateModel) MultiStateModel;

            return new List<SirAgent>
            {
                dummyAgent
            };
        }
    }
}