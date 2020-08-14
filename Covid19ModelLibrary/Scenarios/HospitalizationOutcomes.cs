namespace Covid19ModelLibrary.Scenarios
{
    public class HospitalizationOutcomes : EnumerationWeights<HospitalOutcome>
    {
        public double Recover
        {
            get => GetWeight(HospitalOutcome.Recover);
            set => SetWeight(HospitalOutcome.Recover, value);
        }

        public double GoToIcu
        {
            get => GetWeight(HospitalOutcome.IntensiveCare);
            set => SetWeight(HospitalOutcome.IntensiveCare, value);
        }

        public double Die
        {
            get => GetWeight(HospitalOutcome.Die);
            set => SetWeight(HospitalOutcome.Die, value);
        }

    }
}