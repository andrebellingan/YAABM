using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    internal class TrDieWaitingForHospital : WithinAgentTransition<Human>
    {
        public TrDieWaitingForHospital(CovidStateModel covidStateModel) : base("tr_QIH_to_D", covidStateModel.Qih, covidStateModel.D)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            return agent.NumberOfDaysInCurrentState >= agent.DaysCanSurviveWithoutHospital;
        }
    }
}