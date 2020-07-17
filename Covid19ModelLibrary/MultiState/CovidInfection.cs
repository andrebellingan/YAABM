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
            var p = susceptibleAgent.CovidContext.ProbabilityOfInfection;
            var s = susceptibleAgent.CovidContext.SusceptibilityFactor(susceptibleAgent);

            var probOfTransition = p * s;
            if (double.IsNaN(probOfTransition)) throw new NotFiniteNumberException("Probability of infection is NaN");

            if (probOfTransition < 0d || probOfTransition > 1d) throw new ArgumentOutOfRangeException(nameof(probOfTransition), probOfTransition, "Probability of infection must be in range [0,1]");

            return random.Chance(probOfTransition);
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
