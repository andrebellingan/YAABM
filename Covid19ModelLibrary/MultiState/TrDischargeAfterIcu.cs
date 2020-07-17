using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    internal class TrDischargeAfterIcu : WithinAgentTransition<Human>
    {
        public TrDischargeAfterIcu(CovidStateModel diseaseModel) : base("tr_RICU_to_R", diseaseModel.RIcu, diseaseModel.R)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            var daysInRecovery = agent.NumberOfDaysInCurrentState;

            return daysInRecovery >= agent.DaysRecoveringAfterIcu;
        }
    }
}