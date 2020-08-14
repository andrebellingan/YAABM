using Yaabm.generic;

namespace Covid19ModelLibrary.Interventions
{
    public class IntroduceLockDown: IIntervention
    {
        private readonly int _lockDownLevel;

        public IntroduceLockDown(int lockDownLevel, int day)
        {
            _lockDownLevel = lockDownLevel;
            DayOfIntervention = day;
        }

        public int DayOfIntervention { get; set; }
        public string Description => $"Set all wards to lockdown level {_lockDownLevel}";

        public void Apply(object targetSimulation)
        {
            var simulation = (CovidSimulation) targetSimulation;
            simulation.ApplyLockDownToAllWards(_lockDownLevel);
        }
    }
}
