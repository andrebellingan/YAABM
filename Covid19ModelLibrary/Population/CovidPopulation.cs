using System;
using System.Collections.Generic;
using Serilog;
using Yaabm.generic;
using Yaabm.generic.Random;

namespace Covid19ModelLibrary.Population
{
    public class CovidPopulation : PopulationDynamics<Human>
    {
        public CovidPopulation(int capacity)
        {
            _contactGraph = new ContactGraph(capacity);
            OnAgentAdded += AggAgentToGraphs;
        }

        private void AggAgentToGraphs(Human agent)
        {
            _contactGraph.AddVertex(agent);
        }

        public void AddConnection(Human agent1, Human agent2, ContactSetting setting)
        {
            AddConnection(agent1.Id, agent2.Id, setting);
        }

        public void AddConnection(int agent1Id, int agent2Id, ContactSetting setting)
        {
            _contactGraph.ConnectAgents(agent1Id, agent2Id, new { Setting = setting });
        }

        private readonly ContactGraph _contactGraph;

        protected override Human GenerateNewAgent(int id)
        {
            return new Human(id);
        }

        public override IEnumerable<Encounter<Human>> GetEncounters(Human agent, IRandomProvider random)
        {
            var agentContacts = new List<Encounter<Human>>();

            foreach (var adjacentEdge in _contactGraph.AdjacentEdges(agent.Id))
            {
                var agentId = adjacentEdge.OtherConnectedAgent(agent.Id);
                var encounter = new Encounter<Human>
                {
                    Agent = AgentById(agentId),
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

        public static List<int> SampleWeightedAgents(List<int> candidates, IEnumerable<double> weights, int noOfSamples, IRandomProvider random)
        {
            var numberToSelect = Math.Min(noOfSamples, candidates.Count);

            var selected = Reservoir.ReservoirSampling(weights, numberToSelect, random);

            var result = new List<int>(noOfSamples);

            foreach (var item in selected)
            {
                result.Add(candidates[item]);
            }

            return result;
        }

        public List<int> OtherAgentsInArea(Ward ward, List<int> agentsToExclude)
        {
            var result = new List<int>(ward.ResidentIds.Count);
            foreach (var otherAgentId in ward.ResidentIds)
            {
                if (agentsToExclude.Contains(otherAgentId)) continue;

                result.Add(otherAgentId);
            }

            return result;
        }
    }
}
