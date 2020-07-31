using System.Collections.Generic;

namespace Yaabm.generic
{
    public class Agent<T> : IEqualityComparer<Agent<T>> where T : Agent<T> 
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
            CurrentState = newState;
            DayCurrentStateEntered = day;
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
    }
}