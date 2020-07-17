using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class TrRecoverOutOfHospital : WithinAgentTransition<Human>
    {
        public TrRecoverOutOfHospital(CovidStateModel diseaseModel) : base("tr_I_to_R", diseaseModel.I, diseaseModel.R)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            if (agent.Hospitalization != Hospitalization.None) return false;

            var daysInfected = agent.NumberOfDaysInCurrentState;

            return (daysInfected >= agent.DaysInInfectedState);
        }
    }
}
