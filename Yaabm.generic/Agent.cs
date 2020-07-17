namespace Yaabm.generic
{
    public abstract class Agent<T> where T : Agent<T>
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

        public virtual LocalContext<T> Context { get; set; }

        internal void SetCurrentState(ModelState<T> newState, int day)
        {
            CurrentState = newState;
            DayCurrentStateEntered = day;
        }

        public int NumberOfDaysInCurrentState => Context.Day - DayCurrentStateEntered;

        public abstract void Behave();
    }
}