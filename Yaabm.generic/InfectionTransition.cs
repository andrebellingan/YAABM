namespace Yaabm.generic
{
    public abstract class InfectionTransition<T> : Transition<T> where T : Agent<T>
    {
        protected InfectionTransition(string description, ModelState<T> origin, ModelState<T> destination) :
            base(description, origin, destination, TransitionType.AgentInteraction)
        {
        }

        public abstract bool InfectionOccurs(T carrierAgent, Encounter<T> encounter, IRandomProvider randomProvider);
    }
}