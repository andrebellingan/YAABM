using System;
using System.Collections.Generic;
using Serilog;

namespace Yaabm.generic
{
    public class Agent<TAgent> : IEqualityComparer<Agent<TAgent>>, IComparable<Agent<TAgent>> where TAgent : Agent<TAgent>
    {
        protected Agent(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public ModelState<TAgent> CurrentState { get; private set; }

        public int DayCurrentStateEntered { get; private set; }

        private LocalArea<TAgent> _homeArea;

        public virtual LocalArea<TAgent> HomeArea
        {
            get => _homeArea;
            set
            {
                var previousArea = _homeArea;
                _homeArea = value;
                NotifyHomeAreaChanged((TAgent) this, previousArea);
            }
        }

        private void NotifyHomeAreaChanged(TAgent agent, LocalArea<TAgent> previousArea)
        {
            OnHomeAreaChanged?.Invoke(agent, previousArea);
        }

        internal void SetCurrentState(ModelState<TAgent> newState, int day)
        {
            var previousState = CurrentState;
            CurrentState = newState;
            DayCurrentStateEntered = day;
            TransitionReserved = false;

            NotifyStateChange((TAgent) this, previousState);
        }

        private void NotifyStateChange(TAgent agent, ModelState<TAgent> previousState)
        {
            OnStateChange?.Invoke(agent, previousState);
        }

        public int NumberOfDaysInCurrentState => HomeArea.Day - DayCurrentStateEntered;

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

        public delegate void LocalAreaChangeDelegate(TAgent agent, LocalArea<TAgent> previousArea);

        public StateChangeDelegate OnStateChange { get; set; }

        public LocalAreaChangeDelegate OnHomeAreaChanged { get; set; }

        public bool TransitionReserved { get; internal set; }

        public override string ToString()
        {
            return $"Agent {Id}";
        }
    }
}