using FileHelpers;

namespace Covid19ModelLibrary.Population
{
    [DelimitedRecord("|")]
    internal class ProvincialAgeGroup
    {
        public Province Province { get; set; }

        public AgeBand AgeBand { get; set; }

        public int NumberOfLives { get; set; }
    }
}