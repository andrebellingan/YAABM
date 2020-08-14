using System;
using System.Runtime.Serialization;

namespace Covid19ModelLibrary.Scenarios
{
    [DataContract(Namespace = "https://YACABM/")]
    public class DiseaseParameters
    {
        [DataMember]
        public double ProbInfection { get; set; }

        [DataMember]
        public double Gamma { get; set; }

        [DataMember]
        public double Sigma { get; set; }

        [DataMember]
        public SymptomWeights SymptomWeights { get; set; } = new SymptomWeights();

        [DataMember]
        public double IncubationPeriod { get; set; }

        [DataMember]
        public double TimeInfectious { get; set; }

        [DataMember]
        public double PropStoH { get; set; }

        [DataMember]
        public double TimeStoH { get; set; }

        [DataMember]
        public double TimeStoC { get; set; }

        [DataMember]
        public HospitalizationOutcomes HospitalizationOutcomes { get; set; } = new HospitalizationOutcomes();

        [DataMember]
        public double TimeHtoR { get; set; }

        [DataMember]
        public double TimeHToC { get; set; }

        [DataMember]
        public double TimeHtoD { get; set; }

        [DataMember]
        public double PropDieInIcu { get; set; }

        [DataMember]
        public double TimeCtoD { get; set; }

        [DataMember]
        public double TimeCtoR { get; set; }

        [DataMember]
        public double RelativeBetaAsymptomatic { get; set; }

        [DataMember]
        public double LockdownFactor5 { get; set; }

        [DataMember]
        public double LockdownFactor4 { get; set; }

        [DataMember]
        public double LockdownFactor3 { get; set; }

        [DataMember]
        public double LockdownFactor2 { get; set; }

        [DataMember]
        public double LockdownFactorPost { get; set; }

        public double GetLockDownFactor(in int level)
        {
            return level switch
            {
                5 => LockdownFactor5,
                4 => LockdownFactor4,
                3 => LockdownFactor3,
                2 => LockdownFactor2,
                0 => LockdownFactorPost,
                -1 => 1d,
                _ => throw new ArgumentException($"The factor for lockdown level {level} is not mapped in the scenario")
            };
        }
    }
}