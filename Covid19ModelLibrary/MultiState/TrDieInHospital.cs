using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class TrDieInHospital : WithinAgentTransition<Human>
    {
        public TrDieInHospital(CovidStateModel diseaseModel) : base("tr_IH_to_D", diseaseModel.Ih, diseaseModel.D)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            if (agent.HospitalOutcome != HospitalOutcome.Die) return false;

            var daysInHospital = agent.NumberOfDaysInCurrentState;

            return daysInHospital >= agent.DaysInHospital;
        }
    }
}
