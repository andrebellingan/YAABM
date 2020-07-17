using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    internal class TrDieInIcu : WithinAgentTransition<Human>
    {
        public TrDieInIcu(CovidStateModel diseaseModel) : base("tr_ICU_to_D", diseaseModel.Icu, diseaseModel.D)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            if (agent.IntensiveCareOutcome != IntensiveCareOutcome.Die) return false;

            var daysInIcu = agent.NumberOfDaysInCurrentState;

            return daysInIcu >= agent.DaysInIcu;
        }
    }
}