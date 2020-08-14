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
            CreateConditionalTransition(E, I, agent => agent.NumberOfDaysInCurrentState >= agent.IncubationTime);
            CreateConditionalTransition(I, R, agent => agent.NumberOfDaysInCurrentState >= agent.InfectiousDays);
        }

        private void AgentBecomesInfectious(SirAgent agent, IRandomProvider random)
        {
           agent.InfectiousDays = random.SampleDaysInState(agent.SirContext.GammaParam);
        }

        private void AgentExposed(SirAgent agent, IRandomProvider random)
        {
            agent.IncubationTime = random.SampleDaysInState(agent.SirContext.SigmaParam);
        }

        public override ModelState<SirAgent> DefaultState => S;
    }
}
