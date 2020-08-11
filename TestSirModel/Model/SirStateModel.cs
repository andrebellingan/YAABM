using Yaabm.generic;

namespace TestSirModel.Model
{
    public class SirStateModel : MultiStateModel<SirAgent>
    {
        public ModelState<SirAgent> S { get; }

        public ModelState<SirAgent> E { get; }

        public ModelState<SirAgent> I { get; }

        public ModelState<SirAgent> R { get; }

        public SirStateModel()
        {
            S = CreateModelState("S", false, true);
            E = CreateModelState("E", false, false, AgentExposed);
            I = CreateModelState("I", true, false, AgentBecomesInfectious);
            R = CreateModelState("R", false, false);

            SetInfectionTransition(new InfectionTransition(this));
            // Now the within host transitions
            AddTransition(new IncubationTransition(this));
            AddTransition(new RecoveryTransition(this));
        }

        private void AgentBecomesInfectious(SirAgent agent, IRandomProvider random)
        {
            var mean = 1d / agent.SirContext.GammaParam;
            agent.InfectiousDays = random.SamplePoisson(mean);
        }

        private void AgentExposed(SirAgent agent, IRandomProvider random)
        {
            var p = agent.SirContext.SigmaParam;
            var invP = 1d / p;
            agent.IncubationTime = random.SamplePoisson(invP);
        }

        public override ModelState<SirAgent> DefaultState => S;
    }
}
