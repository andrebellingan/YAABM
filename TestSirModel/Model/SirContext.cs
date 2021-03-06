﻿using Yaabm.generic;

namespace TestSirModel.Model
{
    public class SirContext : LocalArea<SirAgent>
    {
        public int SusceptibleTotal { get; set; }
        public int ExposedTotal { get; set; }
        public int InfectiousTotal { get; set; }
        public int ResistantTotal { get; set; }
        public int N { get; set; }

        public SirContext(string name, SirEnvironment dynamics) : base(name, name, "global", dynamics)
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
