

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


    public class Human : Agent<Human>
    {
        public Human(int id) : base(id) 
        {}

        public AgeBand AgeBand { get; set; } = AgeBand.Age00To04;


        public bool IsInfectious { get; set; }

        public int? IncubationDays { get; set; }


        public Ward Ward { get; private set; }

        private LocalArea<Human> _localContext;

        public override LocalArea<Human> Context
        {
            get => _localContext;
            set
            {
                _localContext = value;
                Ward = value as Ward;
            }
        }
    }
}
 