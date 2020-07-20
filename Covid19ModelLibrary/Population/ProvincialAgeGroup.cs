using System;

namespace Covid19ModelLibrary.Population
{
    internal class ProvincialAgeGroup
    {
        public Province Province { get; set; }

        public AgeBand AgeBand { get; set; }

        public int NumberOfLives { get; set; }

        public static ProvincialAgeGroup FromLine(string nextLine)
        {
            var s = nextLine.Split('|');
            var province = (Province) Enum.Parse(typeof(Province), s[0]);
            var ageBand = (AgeBand) Enum.Parse(typeof(AgeBand), s[1],true);
            var numberOfLives = int.Parse(s[2]);

            return new ProvincialAgeGroup()
            {
                Province = province,
                AgeBand = ageBand,
                NumberOfLives = numberOfLives
            };
        }
    }
}