using System;
using System.Collections.Generic;
using System.Linq;
using Loyc.Collections;
using Serilog;
using Yaabm.generic;
using Yaabm.generic.Random;

namespace Covid19ModelLibrary.Population
{
    public class CovidPopulation : PopulationDynamics<Human>
    {
        public CovidPopulation()
        {
            OnAgentAdded += AggAgentToGraphs;
        }

        private void AggAgentToGraphs(Human agent)
        {
            _contactGraph.AddVertex(agent);
        }

        public void AddConnection(Human agent1, Human agent2, ContactSetting setting)
        {
            _contactGraph.ConnectAgents(agent1, agent2, new {Setting = setting});
        }

        private readonly ContactGraph _contactGraph = new ContactGraph();

        protected override Human GenerateNewAgent(int id)
        {
            return new Human(id);
        }

        public override IEnumerable<Human> GetContacts(Human agent, IRandomProvider random) //TODO: Change this to also be the agent link with the setting type!
        {
            var agentContacts = new List<ContactEdge>();
            agentContacts.AddRange(_contactGraph.AdjacentEdges(agent));

            var result = new List<Human>();
            foreach (var edge in agentContacts)
            {
                result.Add(edge.OtherConnectedAgent(agent));
            }

            return result;
        }

        public void SaveGraphs(in int iterationNo)
        {
            var filename = $"Output/Contact_graph_{iterationNo}.xml";
            ContactGraph.SaveGraphToMl(_contactGraph, filename);
            Log.Verbose("Saved network files");
        }

        public static List<Human> SampleWeightedAgents(List<Tuple<Human, double>> candidates, int noOfSamples, IRandomProvider random)
        {
            var weights = new List<WeightedItem<Human>>();
            var numberToSelect = Math.Min(noOfSamples, candidates.Count);

            foreach (var c in candidates)
            {
                weights.Add(new WeightedItem<Human>(c.Item1, c.Item2));
            }

            var selected = WeightedSampler<Human>.PickMultipleItems(weights, numberToSelect, random);
            return selected;
        }

        public List<Human> OtherAgentsInArea(Ward ward, List<Human> agentsToExclude)
        {
            var result = new List<Human>();
            foreach (var otherAgent in ward.Residents)
            {
                if (agentsToExclude.Contains(otherAgent)) continue;

                result.Add(otherAgent);
            }

            return result;
        }
    }
}
