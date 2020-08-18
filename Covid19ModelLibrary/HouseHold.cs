using System;
using System.Collections.Generic;
using Covid19ModelLibrary.Population;

namespace Covid19ModelLibrary
{
    public class HouseHold
    {
        public int HouseholdHeadId { get; }
        public int HouseholdNumber { get; }

        public List<int> Members { get; } = new List<int>();

        public HouseHold(int headAgentId, int householdNumber)
        {
            HouseholdHeadId = headAgentId;
            HouseholdNumber = householdNumber;
            Members.Add(HouseholdHeadId);
        }

        public void AddContacts(CovidPopulation populationDynamics)
        {
            for (var i = 0; i < Members.Count; i++)
            {
                var agent1 = Members[i];

                for (var j = i + 1; j < Members.Count; j++)
                {
                    var agent2 = Members[j];

                    if (agent1 == agent2)
                    {
                        throw new Exception("This would create a self loop!");
                    }

                    populationDynamics.AddConnection(agent1, agent2, ContactSetting.Home);
                }
            }
        }
    }
}
