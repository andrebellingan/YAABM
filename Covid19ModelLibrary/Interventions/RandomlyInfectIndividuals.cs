using Yaabm.generic;

namespace Covid19ModelLibrary.Interventions
{
    // ReSharper disable once UnusedMember.Global
    public class RandomlyInfectIndividuals : IIntervention
    {
        private readonly int _numberToInfect;

        public RandomlyInfectIndividuals(int numberToInfect)
        {
            _numberToInfect = numberToInfect;
        }

        public int DayOfIntervention { get; set; }

        public string Description => $"Randomly infect {_numberToInfect} agents";

        public void Apply(object targetSimulation)
        {
            var simulation = (CovidSimulation) targetSimulation;
            simulation.RandomlyInfect(_numberToInfect);
        }
    }
}