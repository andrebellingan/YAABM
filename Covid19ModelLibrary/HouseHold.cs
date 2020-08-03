using System.Collections.Generic;

namespace Covid19ModelLibrary
{
    public class HouseHold
    {
        public Human HouseholdHead { get; }
        public int HouseholdNumber { get; }

        public List<Human> Members { get; } = new List<Human>();

        public HouseHold(Human headAgent, int householdNumber)
        {
            HouseholdHead = headAgent;
            HouseholdNumber = householdNumber;
            Members.Add(HouseholdHead);
        }
    }
}
