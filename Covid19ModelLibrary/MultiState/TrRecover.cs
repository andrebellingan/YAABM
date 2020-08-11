using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    internal class TrRecover : WithinAgentTransition<Human>
    {
        public TrRecover(CovidStateModel covidStateModel) : base("I->R", covidStateModel.I, covidStateModel.R)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            return random.Chance(agent.Ward.DiseaseParameters.Gamma);
        }
    }
}