using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    internal class TrRecover : WithinAgentTransition<Human>
    {
        private CovidStateModel covidStateModel;

        public TrRecover(CovidStateModel covidStateModel) : base("Recover", covidStateModel.I, covidStateModel.R)
        {
            this.covidStateModel = covidStateModel;
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            throw new System.NotImplementedException(nameof(TransitionOccurs));
        }
    }
}