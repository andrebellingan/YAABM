using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    internal class TrExitIcu : WithinAgentTransition<Human>
    {
        public TrExitIcu(CovidStateModel diseaseModel) : base("tr_ICU_to_RICU", diseaseModel.Icu, diseaseModel.RIcu)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            if (agent.IntensiveCareOutcome != IntensiveCareOutcome.Recover) return false;

            var daysSpentInIcu = agent.NumberOfDaysInCurrentState;

            return daysSpentInIcu >= agent.DaysInIcu;
        }
    }
}