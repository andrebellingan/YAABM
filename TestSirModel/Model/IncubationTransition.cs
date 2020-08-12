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
            return agent.NumberOfDaysInCurrentState >= agent.IncubationTime;
        }
    }
}