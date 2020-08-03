using System;
using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class CovidInfection : InfectionTransition<Human>
    {
        public CovidInfection(CovidStateModel stateModel) : base("tr_S_to_E", stateModel.S, stateModel.E)
        {
        }

        public override bool InfectionOccurs(Human carrierAgent, Human susceptibleAgent, IRandomProvider random)
        {
            throw new NotImplementedException(nameof(InfectionOccurs));
        }

        public override bool IsInfectionSource(Human agent)
        {
            return agent.IsInfectious = true;
        }

        public override bool IsSusceptible(Human agent)
        {
            return agent.CurrentState == Origin; // origin is S (from constructor)
        }
    }
}
