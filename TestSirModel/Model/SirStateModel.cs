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
            S = CreateModelState("S");
            E = CreateModelState("E");
            I = CreateModelState("I", AgentInfected);
            R = CreateModelState("R", AgentRecovered);

            SetInfectionTransition(new InfectionTransition(this));
            // Now the within host transitions
            AddTransition(new IncubationTransition(this));
            AddTransition(new RecoveryTransition(this));
        }

        private void AgentRecovered(SirAgent agent, IRandomProvider random)
        {
            agent.IsInfectious = false;
        }

        private void AgentInfected(SirAgent agent, IRandomProvider random)
        {
            agent.IsInfectious = true;
        }

        public override ModelState<SirAgent> DefaultState => S;
    }
}
