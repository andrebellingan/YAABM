using System;

namespace Yaabm.generic
{
    public class ConditionalTransition<TAgent> : WithinAgentTransition<TAgent> where TAgent : Agent<TAgent>
    {
        private readonly Func<TAgent, bool> _conditionFunction;

        public ConditionalTransition(ModelState<TAgent> origin, ModelState<TAgent> destination, Func<TAgent, bool> condition) : base($"{origin.Name}_to_{destination.Name}", origin, destination)
        {
            _conditionFunction = condition;
        }

        public override bool TransitionOccurs(TAgent agent, IRandomProvider random)
        {
            return _conditionFunction(agent);
        }
    }
}
