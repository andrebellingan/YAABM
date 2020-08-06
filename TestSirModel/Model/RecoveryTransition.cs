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
            if (!(agent.HomeArea is SirContext asSirContext)) throw new InvalidCastException("Context must be an instance of SirContext");

            var probabilityOfRecovery = asSirContext.GammaParam; // Convert from rate to single day
            return random.Chance(probabilityOfRecovery);
        }
    }
}