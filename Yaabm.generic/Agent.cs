using System;
using System.Collections.Generic;

namespace Yaabm.generic
{
    public class Agent<TAgent> : IEqualityComparer<Agent<TAgent>>, IComparable<Agent<TAgent>> where TAgent : Agent<TAgent>
    {
        protected Agent(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }

        internal void SetId(int id)
        {
            Id = id;
        }

        public ModelState<TAgent> CurrentState { get; private set; }

        public int DayCurrentStateEntered { get; private set; }

        public virtual LocalArea<TAgent> Context { get; set; }

        internal void SetCurrentState(ModelState<TAgent> newState, int day)
        {
            var previousState = CurrentState;
            CurrentState = newState;
            DayCurrentStateEntered = day;

            NotifyStateChange((TAgent) this, previousState);
        }

        private void NotifyStateChange(TAgent agent, ModelState<TAgent> previousState)
        {
            OnStateChange?.Invoke(agent, previousState);
        }

        public int NumberOfDaysInCurrentState => Context.Day - DayCurrentStateEntered;

        public virtual void Behave()
        {
            //Default behaviour is to do nothing
        }

        public bool Equals(Agent<TAgent> x, Agent<TAgent> y)
        {
            return y != null && (x != null && x.Id == y.Id);
        }

        public int GetHashCode(Agent<TAgent> agent)
        {
            return Id;
        }

        public int CompareTo(Agent<TAgent> other)
        {
            return Id.CompareTo(other.Id);
        }

        public delegate void StateChangeDelegate(TAgent agent, ModelState<TAgent> previousState);

        public StateChangeDelegate OnStateChange { get; set; }
    }
}