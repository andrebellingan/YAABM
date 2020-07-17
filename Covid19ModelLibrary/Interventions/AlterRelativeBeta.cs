using System;
using Yaabm.generic;

namespace Covid19ModelLibrary.Interventions
{
    public class AlterRelativeBeta : IIntervention
    {
        public AlterRelativeBeta(double newRelativeBeta, string description)
        {
            RelativeBeta = newRelativeBeta;
            Description = description;
        }

        public double RelativeBeta { get; set; }

        public int DayOfIntervention { get; set; }

        public string Description { get; }

        public void Apply(object targetSimulation)
        {
            if (!(targetSimulation is CovidSimulation covidSim))
            {
                InternalLog.Error($"Cannot apply intervention \"{Description}\" because simulation is null or not of type CovidSimulation");
                throw new InvalidOperationException("Cannot apply intervention to null simulation");
            }

            covidSim.ApplyChangeToAllContexts(c => c.RelativeLockDownBeta = RelativeBeta);
        }
    }
}
