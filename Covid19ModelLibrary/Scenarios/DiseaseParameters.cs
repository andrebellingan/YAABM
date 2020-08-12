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
    }
}