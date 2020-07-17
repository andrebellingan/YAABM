namespace Yaabm.generic
{
    public abstract class InfectionTransition<T> : Transition<T> where T : Agent<T>
    {
        protected InfectionTransition(string description, ModelState<T> origin, ModelState<T> destination) :
            base(description, origin, destination, TransitionType.AgentInteraction)
        {
        }

        public abstract bool InfectionOccurs(T carrierAgent, T susceptibleAgent, IRandomProvider randomProvider);

        public abstract bool IsInfectionSource(T agent); // keep this for later when using the contact model

        public abstract bool IsSusceptible(T agent);
    }
}