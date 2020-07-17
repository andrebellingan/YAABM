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

        public override bool InfectionOccurs(SirAgent carrierAgent, SirAgent susceptibleAgent, IRandomProvider random)
        {
            var sirContext = (SirContext)susceptibleAgent.Context;

            var p = sirContext.ProbabilityOfInfection;

            if (p < 0d || p > 1d) throw new InvalidOperationException("Probability must be in [0, 1]");

            return random.Chance(p);
        }

        public override bool IsInfectionSource(SirAgent agent)
        {
            return agent.IsInfectious;
        }

        public override bool IsSusceptible(SirAgent agent)
        {
            return agent.CurrentState == StateModel.S;
        }
    }
}