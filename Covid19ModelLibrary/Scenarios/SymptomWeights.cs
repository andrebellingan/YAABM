namespace Covid19ModelLibrary.Scenarios
{
    public class SymptomWeights : EnumerationWeights<Symptoms>
    {
        public double Asymptomatic
        {
            get => GetWeight(Symptoms.Asymptomatic);
            set => SetWeight(Symptoms.Asymptomatic, value);
        }

        public double Mild
        {
            get => GetWeight(Symptoms.Mild);
            set => SetWeight(Symptoms.Mild, value);
        }

        public double Severe
        {
            get => GetWeight(Symptoms.Severe);
            set => SetWeight(Symptoms.Severe, value);
        }
    }
}
