namespace Cabm.Core
{
    public interface ITransition<TAgent> where TAgent : Agent<TAgent>
    {
        string Description { get; }

        ModelState<TAgent> Origin { get; }

        ModelState<TAgent> Destination { get; }

        TransitionType TransitionType { get; }
    }
}