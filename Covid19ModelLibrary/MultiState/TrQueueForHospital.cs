using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    internal class TrQueueForHospital : WithinAgentTransition<Human>
    {
        public TrQueueForHospital(CovidStateModel covidStateModel) : base("tr_I_to_QIH", covidStateModel.I, covidStateModel.Qih)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            if (agent.Hospitalization != Hospitalization.Hospital) return false;
            if (agent.NumberOfDaysInCurrentState < agent.DaysInInfectedState) return false;

            return !agent.CovidContext.HospitalSystem.HospitalBedIsAvailable(agent, false);
        }
    }
}