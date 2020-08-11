using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    internal class TrRecover : WithinAgentTransition<Human>
    {
        public TrRecover(CovidStateModel covidStateModel) : base("Recover", covidStateModel.I, covidStateModel.R)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            return random.Chance(agent.Ward.DiseaseParameters.Gamma);
        }
    }
}