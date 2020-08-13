using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class Infection : InfectionTransition<Human>
    {
        private readonly CovidStateModel _stateModel;

        public Infection(CovidStateModel stateModel) : base("S_to_E", stateModel.S, stateModel.E)
        {
            _stateModel = stateModel;
        }

        public override bool InfectionOccurs(Human carrierAgent, Encounter<Human> encounter, IRandomProvider randomProvider)
        {
            return randomProvider.Chance(_stateModel.Parameters.ProbInfection);
        }

    }
}
