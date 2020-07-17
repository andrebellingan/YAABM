using System;
using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class TrEndIncubationPeriod : WithinAgentTransition<Human>
    {
        public TrEndIncubationPeriod(CovidStateModel diseaseModel) : base("tr_E_to_I", diseaseModel.E, diseaseModel.I)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            var daysInState = agent.NumberOfDaysInCurrentState;

            if (agent.IncubationDays == null) throw new Exception("Incubation period has not been set for this agent");

            return (daysInState > agent.IncubationDays);
        }
    }
}
