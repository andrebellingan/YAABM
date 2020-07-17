using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class TrRecoverInHospital : WithinAgentTransition<Human>
    {
        public TrRecoverInHospital(CovidStateModel diseaseModel) : base("tr_IH_to_R", diseaseModel.Ih, diseaseModel.R)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            if (agent.HospitalOutcome != HospitalOutcome.Recover) return false;

            var daysInHospital = agent.NumberOfDaysInCurrentState;

            return (daysInHospital >= agent.DaysInHospital);
        }
    }
}
