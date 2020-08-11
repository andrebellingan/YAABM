using Yaabm.generic;

namespace Covid19ModelLibrary.MultiState
{
    public class TrEndIncubationPeriod : WithinAgentTransition<Human>
    {
        public TrEndIncubationPeriod(CovidStateModel diseaseModel) : base("E->I", diseaseModel.E, diseaseModel.I)
        {
        }

        public override bool TransitionOccurs(Human agent, IRandomProvider random)
        {
            return random.Chance(agent.Ward.DiseaseParameters.Sigma);
        }
    }
}
