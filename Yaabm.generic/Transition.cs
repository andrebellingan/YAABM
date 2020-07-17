namespace Yaabm.generic
{
    public enum TransitionType
    {
        WithinAgent,
        AgentInteraction
    }

    public abstract class Transition<T> where T : Agent<T>
    {
        protected Transition(string description, ModelState<T> origin, ModelState<T> destination,
            TransitionType transitionType)
        {
            Description = description;
            Origin = origin;
            Destination = destination;
            TransitionType = transitionType;
        }

        public string Description { get; }

        public ModelState<T> Origin { get; }

        public ModelState<T> Destination { get; }

        public TransitionType TransitionType { get; }

        public override string ToString()
        {
            return Description;
        }
    }
}