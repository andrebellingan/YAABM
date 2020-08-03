using System;
using System.Collections.Generic;
using Serilog;
using Yaabm.generic;

namespace Covid19ModelLibrary.Population
{
    public class CovidPopulation : PopulationDynamics<Human>
    {
        public CovidPopulation()
        {
            var allSettings = (ContactSetting[]) Enum.GetValues(typeof(ContactSetting));
            foreach (var setting in allSettings)
            {
                _graphs.Add(setting, new ContactGraph(setting));
            }

            OnAgentAdded += AggAgentToGraphs;
        }

        private void AggAgentToGraphs(Human agent)
        {
            foreach (var g in _graphs.Values)
            {
                g.AddVertex(agent);
            }
        }

        public void AddConnection(ContactSetting setting, Human agent1, Human agent2)
        {
            var contactGraph = _graphs[setting];
            contactGraph.ConnectAgents(agent1, agent2);
        }

        private readonly Dictionary<ContactSetting, ContactGraph> _graphs = new Dictionary<ContactSetting, ContactGraph>();

        protected override Human GenerateNewAgent(int id)
        {
            return new Human(id);
        }

        public override IEnumerable<Human> GetContacts(Human agent, IRandomProvider random)
        {
            //TODO: Actually get the agent's contacts - need to implement a contact model first
            return EnumeratePopulation(random, false);
        }

        public override IEnumerable<Human> GetInfectiousAgents()
        {
            throw new NotImplementedException(nameof(GetInfectiousAgents));
        }

        public void SaveGraphs(in int iterationNo)
        {
            foreach (var g in _graphs.Values)
            {
                var filename = $"Output/graph_{iterationNo}_{g.Setting}.xml";
                ContactGraph.SaveGraphToMl(g, filename);
            }
            Log.Verbose("Saved network files");
        }
    }
}
