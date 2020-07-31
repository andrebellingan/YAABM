using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class TrAdmitToIcu : WithinAgentTransition<Human>
    {
        public TrAdmitToIcu(CovidStateModel disease) : base("tr_I_to_ICU", disease.I, disease.Icu)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            if (agent.Hospitalization != Hospitalization.IntensiveCare) return false;

            var daysInfected = agent.NumberOfDaysInCurrentState;
            if (daysInfected < agent.DaysInInfectedState) return false;

            return agent.Ward.HospitalSystem.IcuBedIsAvailable();
        }
    }
}
