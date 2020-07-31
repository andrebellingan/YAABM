using Yaabm.generic;

namespace TestSirModel.Model
{
    public class SirContext : LocalArea<SirAgent>
    {
        public int SusceptibleTotal { get; set; }
        public int ExposedTotal { get; set; }
        public int InfectiousTotal { get; set; }
        public int ResistantTotal { get; set; }
        public int N { get; set; }

        public SirContext(ILocalResourceSystem<SirAgent> localResource, string name) : base(localResource, name, name, "SirContext")
        {
        }

        internal void SetDiseaseParameters(double beta, double gamma, double sigma)
        {
            BetaParam = beta;
            GammaParam = gamma;
            SigmaParam = sigma;
        }

        public double GammaParam { get; private set; }

        public double BetaParam { get; private set; }

        public double SigmaParam { get; private set; }
        public double ProbabilityOfInfection { get; set; }
    }
}
