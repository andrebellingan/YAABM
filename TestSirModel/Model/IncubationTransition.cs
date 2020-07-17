using System;
using Yaabm.generic;

namespace TestSirModel.Model
{
    internal class IncubationTransition : WithinAgentTransition<SirAgent>
    {
        public IncubationTransition(SirStateModel sirStateModel) : base("E->I", sirStateModel.E, sirStateModel.I)
        {
        }

        public override bool TransitionOccurs(SirAgent agent, IRandomProvider random)
        {
            if (!(agent.Context is SirContext asSirContext)) throw new InvalidCastException("Context must be an instance of SirContext");

            var probabilityOfDevelopingSymptoms = asSirContext.SigmaParam;
            return random.Chance(probabilityOfDevelopingSymptoms);
        }
    }
}