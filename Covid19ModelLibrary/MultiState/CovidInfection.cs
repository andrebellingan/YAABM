using System;
using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class CovidInfection : InfectionTransition<Human>
    {
        public CovidInfection(CovidStateModel stateModel) : base("S_to_E", stateModel.S, stateModel.E)
        {
        }

        public override bool InfectionOccurs(Human carrierAgent, Encounter<Human> encounter, IRandomProvider randomProvider)
        {
            return randomProvider.Chance(carrierAgent.Ward.DiseaseParameters.ProbInfection);
        }

    }
}
