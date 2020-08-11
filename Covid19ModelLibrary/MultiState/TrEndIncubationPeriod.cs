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
            return random.Chance(agent.Ward.DiseaseParameters.Sigma);
        }
    }
}
