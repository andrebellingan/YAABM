using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class TrAdmitToHospital : WithinAgentTransition<Human>
    {
        public TrAdmitToHospital(CovidStateModel diseaseModel) : base("tr_I_to_IH", diseaseModel.I, diseaseModel.Ih)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            if (agent.Hospitalization != Hospitalization.Hospital) return false;

            var daysInfected = agent.NumberOfDaysInCurrentState;
            if (daysInfected < agent.DaysInInfectedState) return false;

            return agent.CovidContext.HospitalSystem.HospitalBedIsAvailable(agent, true);
        }
    }
}
