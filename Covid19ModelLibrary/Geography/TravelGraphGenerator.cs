using Covid19ModelLibrary.Population;
using System.Collections.Generic;
using System.Linq;
using Yaabm.generic;

namespace Covid19ModelLibrary.Geography
{
    public class TravelGraphGenerator
    {
        public void GenerateGraph(CovidPopulation populationDynamics, IEnumerable<Ward> localAreas,
            TravelMatrix travelMatrix, ContactMatrix contactMatrix, IRandomProvider random)
        {
            var allAgents = populationDynamics.EnumeratePopulation();

            foreach (var agent in allAgents)
            {
                var travelWardId = travelMatrix.RandomDestination(agent.WardId, random);
                var destinationWard = localAreas.First(p => p.WardId == travelWardId);
                var potentialContacts = populationDynamics.OtherAgentsInArea(destinationWard, agent.HouseHold.Members);
                if (potentialContacts.Count > 0)
                {
                    var p = contactMatrix.ContactWeightedAgentList(potentialContacts, populationDynamics, agent);
                    var numberOfContacts = contactMatrix.SampleNoOfContacts(agent, random);
                    var selected = CovidPopulation.SampleWeightedAgents(potentialContacts, p, numberOfContacts, random);

                    foreach (var contact in selected)
                    {
                        populationDynamics.AddConnection(agent.Id, contact, ContactSetting.Other);
                    }
                }

            }
        }
    }
}
