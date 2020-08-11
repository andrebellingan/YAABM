using System;
using Yaabm.generic;

namespace TestSirModel.Model
{
    internal class InfectionTransition : InfectionTransition<SirAgent>
    {
        public InfectionTransition(SirStateModel sirStateModel) : base("S->E", sirStateModel.S, sirStateModel.E)
        {
            StateModel = sirStateModel;
        }

        public SirStateModel StateModel { get; }

        public override bool InfectionOccurs(SirAgent carrierAgent, Encounter<SirAgent> encounter, IRandomProvider random)
        {
            var sirContext = (SirContext) encounter.Agent.HomeArea;

            var p = sirContext.ProbabilityOfInfection;

            if (p < 0d || p > 1d) throw new InvalidOperationException("Probability must be in [0, 1]");

            return random.Chance(p);
        }
    }
}