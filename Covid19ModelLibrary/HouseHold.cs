using System;
using System.Collections.Generic;
using Covid19ModelLibrary.Population;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class HouseHold
    {
        public Human HouseholdHead { get; }
        public int HouseholdNumber { get; }

        public List<Human> Members { get; } = new List<Human>();

        public HouseHold(Human headAgent, int householdNumber)
        {
            HouseholdHead = headAgent;
            HouseholdNumber = householdNumber;
            Members.Add(HouseholdHead);
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

                    populationDynamics.AddConnection(ContactSetting.Home, agent1, agent2);
                }
            }
        }
    }
}
