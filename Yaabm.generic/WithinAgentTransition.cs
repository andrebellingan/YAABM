namespace Yaabm.generic
{
    public abstract class WithinAgentTransition<T> : Transition<T> where T : Agent<T>
    {
        protected WithinAgentTransition(string description, ModelState<T> origin, ModelState<T> destination) : base(
            description, origin, destination, TransitionType.WithinAgent)
        {
        }

        public abstract bool TransitionOccurs(T agent, IRandomProvider random);
    }
}