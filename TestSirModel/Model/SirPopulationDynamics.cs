using System.Collections.Generic;
using Yaabm.generic;

namespace TestSirModel.Model
{
    public class SirPopulationDynamics : PopulationDynamics<SirAgent>
    {
        protected override SirAgent GenerateNewAgent(int id)
        {
            return new SirAgent(id);
        }

        public override IEnumerable<SirAgent> GetContacts(SirAgent agent, IRandomProvider random)
        {
            return EnumeratePopulation(random, false);
        }

        public override IEnumerable<SirAgent> GetInfectiousAgents()
        {
            // Just return a dummy agent since the SEIR model doesn't do real interaction
            return new[] {new SirAgent(99999999)};
        }
    }
}