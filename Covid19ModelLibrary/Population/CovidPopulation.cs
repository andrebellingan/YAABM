using System.Collections.Generic;
using Covid19ModelLibrary.MultiState;
using Yaabm.generic;

namespace Covid19ModelLibrary.Population
{
    public class CovidPopulation : PopulationDynamics<Human>
    {
        public override IEnumerable<Human> GetContacts(Human agent, IRandomProvider random)
        {
            //TODO: Actually get the agent's contacts - need to implement a contact model first
            return EnumeratePopulation(random, false);
        }

        public override IEnumerable<Human> GetInfectiousAgents()
        {
            //TODO: Actually enumerate agents - this will only be required once a contact model has been implemented
            return new[]
            {
                new Human() {IsInfectious = true}
            };
        }

        public int GetInfectiousBySymptoms(DiseaseSymptoms symptoms, Province province)
        {
            var total = 0;

            var diseaseModel = (CovidStateModel) MultiStateModel;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var agent in EnumeratePopulation(null, false))
            {
                if (agent.CurrentState != diseaseModel.I) continue;

                if (agent.Ward.Province == province && agent.Symptoms == symptoms) total++;
            }

            return total;
        }

        public void GenerateContacts(ContactMatrix contactMatrix)
        {
            
        }
    }
}
