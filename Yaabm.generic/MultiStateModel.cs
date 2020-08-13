using System;
using System.Collections.Generic;

namespace Yaabm.generic
{
    public abstract class MultiStateModel<T> where T : Agent<T>
    {
        private readonly List<Transition<T>> _allTransitions = new List<Transition<T>>();
        private readonly List<ModelState<T>> _modelStates = new List<ModelState<T>>();

        public InfectionTransition<T> InfectionTransition { get; private set; }

        public void SetInfectionTransition(InfectionTransition<T> infectionTransition)
        {
            if (_allTransitions.Contains(InfectionTransition)) _allTransitions.Remove(InfectionTransition);
            InfectionTransition = infectionTransition;
            _allTransitions.Add(infectionTransition);
        }

        public IEnumerable<ModelState<T>> States => _modelStates;

        public IEnumerable<Transition<T>> AllTransitions => _allTransitions;
        public abstract ModelState<T> DefaultState { get; }

        internal Transition<T> DetermineWithinHostStateTransitions(T agent, IRandomProvider random, bool shuffle)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var transition in GetWithinHostTransitionsFromState(agent.CurrentState, shuffle, random))
            {
                var transitionOccurs = transition.TransitionOccurs(agent, random);

                if (!transitionOccurs) continue;

                return transition; // only one transition can happen per cycle
            }

            return null;
        }


        public Transition<T> DetermineAgentInteractionTransition(T agent, Encounter<T> encounter, IRandomProvider randomProvider)
        {
            var transitionOccurs = InfectionTransition.InfectionOccurs(agent, encounter, randomProvider);

            return !transitionOccurs ? null : InfectionTransition;
        }

        private static IEnumerable<WithinAgentTransition<T>> GetWithinHostTransitionsFromState(ModelState<T> fromState, bool shuffle, IRandomProvider random)
        {
            return fromState.GetOutgoingWithinHostTransitions(shuffle, random);
        }

        protected ModelState<T> CreateModelState(string name, bool isInfectiousState, bool isSusceptibleState)
        {
            var newState = new ModelState<T>(name, isInfectiousState, isSusceptibleState);
            _modelStates.Add(newState);

            return newState;
        }

        protected ModelState<T> CreateModelState(string name, bool isInfectiousState, bool isSusceptibleState, ModelState<T>.StateEnteredDelegate stateEntered)
        {
            var newState = CreateModelState(name, isInfectiousState, isSusceptibleState);
            newState.OnStateEntered = stateEntered;

            return newState;
        }

        protected void AddTransition(WithinAgentTransition<T> transition)
        {
            if (!_modelStates.Contains(transition.Origin))
                throw new ArgumentException(
                    "This transition is invalid because the origin state is not part of this model");
            if (!_modelStates.Contains(transition.Destination))
                throw new ArgumentException(
                    "This transition is invalid because the destination state is not part of this model");

            _allTransitions.Add(transition);
            transition.Origin.TransitionsFromThisState.Add(transition);

            transition.Origin.WithinHostTransitionsFromState.Add(transition);
        }

        protected void CreateConditionalTransition(ModelState<T> origin, ModelState<T> destination,
            Func<T, bool> transitionCondition)
        {
            var transition = new ConditionalTransition<T>(origin, destination, transitionCondition);
            AddTransition(transition);
        }
    }
}