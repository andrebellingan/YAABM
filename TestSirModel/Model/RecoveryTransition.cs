using System;
using Yaabm.generic;

namespace TestSirModel.Model
{
    internal class RecoveryTransition : WithinAgentTransition<SirAgent>
    {
        public RecoveryTransition(SirStateModel sirStateModel) : base("I->R", sirStateModel.I, sirStateModel.R)
        {
        }

        public override bool TransitionOccurs(SirAgent agent, IRandomProvider random)
        {
            return random.Chance(agent.SirContext.GammaParam);
        }
    }
}