

using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public enum AgeBand
    {
        Age00To04 = 0,
        Age05To09 = 1,
        Age10To14 = 2,
        Age15To19 = 3,
        Age20To24 = 4,
        Age25To29 = 5,
        Age30To34 = 6,
        Age35To39 = 7,
        Age40To44 = 8,
        Age45To49 = 9,
        Age50To54 = 10,
        Age55To59 = 11,
        Age60To64 = 12,
        Age65To69 = 13,
        Age70To74 = 14,
        Age75To79 = 15,
        Age80Plus = 16
    }

    public enum DiseaseSymptoms
    {
        None,
        Incubating,
        Asymptomatic,
        Mild,
        Severe,
    }

    public enum Hospitalization
    {
        None,
        Hospital,
        IntensiveCare
    }

    public enum HospitalOutcome
    {
        Recover,
        MoveToIntensiveCare,
        Die
    }

    public enum IntensiveCareOutcome
    {
        Recover,
        Die
    }

    public class Human : Agent<Human>
    {
        public AgeBand AgeBand { get; set; } = AgeBand.Age00To04;

        public DiseaseSymptoms Symptoms { get; set; } = DiseaseSymptoms.None;

        public Hospitalization Hospitalization { get; set; } = Hospitalization.None;

        public HospitalOutcome HospitalOutcome { get; set; } = HospitalOutcome.Recover;

        public IntensiveCareOutcome IntensiveCareOutcome { get; set; } = IntensiveCareOutcome.Recover;

        public bool IsInfectious { get; set; }

        public int? IncubationDays { get; set; }

        public int? DaysInInfectedState { get; set; }

        public int? DaysInHospital { get; set; }

        public int? DaysInIcu { get; set; }

        public int? DaysRecoveringAfterIcu { get; set; }

        public bool IsAlive { get; set; }

        public CovidContext CovidContext { get; private set; }

        private LocalArea<Human> _localContext;

        public override LocalArea<Human> Context
        {
            get => _localContext;
            set
            {
                _localContext = value;
                CovidContext = value as CovidContext;
            }
        }

        public int? DaysCanSurviveWithoutHospital { get; set; }
    }
}
 