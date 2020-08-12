using System;
using System.Collections.Generic;
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

        public override IEnumerable<Encounter<Human>> GetEncounters(Human agent, IRandomProvider random)
        {
            var agentContacts = new List<Encounter<Human>>();

            foreach (var adjacentEdge in _contactGraph.AdjacentEdges(agent))
            {
                var encounter = new Encounter<Human>
                {
                    Agent = adjacentEdge.OtherConnectedAgent(agent),
                    EncounterInformation = new {Setting = adjacentEdge.ContactSetting}
                };

                agentContacts.Add(encounter);
            }

            return agentContacts;
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
