using Yaabm.generic;

namespace UnitTests.Yaabm.generic
{
    public class SimulationThingy
    {
        public string Result { get; set; }
    }

    public class LabInterventionSpec : IIntervention
    {
        public LabInterventionSpec(double value1, int value2, string value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }

        public double Value1 { get; set; }

        public int Value2 { get; set; }

        public string Value3 { get; set; }

        public int DayOfIntervention { get; set; }

        public string Description => "Lab testing intervention";

        public void Apply(object targetSimulation)
        {
            var asThingy = (SimulationThingy) targetSimulation;
            asThingy.Result = "It worked!";
        }
    }
}
