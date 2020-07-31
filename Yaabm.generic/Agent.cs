using System;
using System.Collections.Generic;

namespace Yaabm.generic
{
    public class Agent<T> : IEqualityComparer<Agent<T>>, IComparable<Agent<T>> where T : Agent<T> 
    {
        protected Agent()
        {
        }

        protected Agent(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }

        internal void SetId(int id)
        {
            Id = id;
        }

        public ModelState<T> CurrentState { get; private set; }

        public int DayCurrentStateEntered { get; private set; }

        public virtual LocalArea<T> Context { get; set; }

        internal void SetCurrentState(ModelState<T> newState, int day)
        {
            var previousState = CurrentState;
            CurrentState = newState;
            DayCurrentStateEntered = day;

            NotifyStateChange(this, previousState);
        }

        private void NotifyStateChange(Agent<T> agent, ModelState<T> previousState)
        {
            OnStateChange?.Invoke(agent, previousState);
        }

        public int NumberOfDaysInCurrentState => Context.Day - DayCurrentStateEntered;

        public virtual void Behave()
        {
            //Default behaviour is to do nothing
        }

        public bool Equals(Agent<T> x, Agent<T> y)
        {
            return y != null && (x != null && x.Id == y.Id);
        }

        public int GetHashCode(Agent<T> agent)
        {
            return Id;
        }

        public int CompareTo(Agent<T> other)
        {
            return Id.CompareTo(other.Id);
        }

        public delegate void StateChangeDelegate(Agent<T> agent, ModelState<T> previousState);

        public StateChangeDelegate OnStateChange { get; set; }
    }
}