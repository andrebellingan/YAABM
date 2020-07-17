using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class TrMovedToIcu : WithinAgentTransition<Human>
    {
        public TrMovedToIcu(CovidStateModel diseaseModel) : base("tr_IH_to_ICU", diseaseModel.Ih, diseaseModel.Icu)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            if (agent.HospitalOutcome != HospitalOutcome.MoveToIntensiveCare) return false;

            var daysInNormalBed = agent.NumberOfDaysInCurrentState;

            return (daysInNormalBed >= agent.DaysInHospital);
        }
    }
}
