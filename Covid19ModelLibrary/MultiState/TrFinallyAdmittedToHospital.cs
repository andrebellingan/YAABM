using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    internal class TrFinallyAdmittedToHospital : WithinAgentTransition<Human>
    {
        public TrFinallyAdmittedToHospital(CovidStateModel covidStateModel) : base("tr_QIH_to_IH", covidStateModel.Qih, covidStateModel.Ih)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            if (agent.NumberOfDaysInCurrentState < agent.DaysCanSurviveWithoutHospital) return false;

            return agent.Ward.HospitalSystem.HospitalBedIsAvailable(agent, true);
        }
    }
}