using System;
using System.Collections.Generic;
using Yaabm.generic;

namespace Covid19ModelLibrary.Population
{
    public class CovidPopulation : PopulationDynamics<Human>
    {
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
    }
}
