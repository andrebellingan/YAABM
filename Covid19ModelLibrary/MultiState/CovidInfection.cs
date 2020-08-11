using System;
using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class CovidInfection : InfectionTransition<Human>
    {
        public CovidInfection(CovidStateModel stateModel) : base("tr_S_to_E", stateModel.S, stateModel.E)
        {
        }

        public override bool InfectionOccurs(Human carrierAgent, Encounter<Human> encounter, IRandomProvider randomProvider)
        {
            var beta = carrierAgent.Ward.DiseaseParameters.RZero * carrierAgent.Ward.DiseaseParameters.Gamma;
            return randomProvider.Chance(beta);
        }

    }
}
