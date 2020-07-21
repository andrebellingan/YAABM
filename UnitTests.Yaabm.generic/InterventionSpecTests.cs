using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yaabm.generic;

namespace UnitTests.Yaabm.generic
{
    [TestClass]
    public class InterventionSpecTests
    {
        [TestMethod]
        public void TestInterventionSpec1()
        {
            var spec = GenerateInterventionSpec();

            var intervention = spec.CreateInstance();

            Assert.IsNotNull(intervention);

            Assert.AreEqual("Lab testing intervention", intervention.Description);
            Assert.AreEqual(180, intervention.DayOfIntervention);

            var asLab = (LabInterventionSpec) intervention;
            Assert.AreEqual(3.1415d, asLab.Value1);
            Assert.AreEqual(42, asLab.Value2);
            Assert.AreEqual("Prefect", asLab.Value3);

            var simulationThing = new SimulationThingy {Result = "DID NOT WORK"};

            intervention.Apply(simulationThing);
            Assert.AreEqual("It worked!", simulationThing.Result);
        }

        private InterventionSpec GenerateInterventionSpec()
        {
            var newSpec = new InterventionSpec
            {
                InterventionName = "UnitTests.Yaabm.generic.LabInterventionSpec, UnitTests.Yaabm.generic",
                DayToApply = 180
            };
            newSpec.Parameters.Add(new InterventionParam() {Name = "Value1", TypeName = "System.Double", Value = "3.1415"});
            newSpec.Parameters.Add(new InterventionParam() { Name = "Value2", TypeName = "System.Int32", Value = "42" });
            newSpec.Parameters.Add(new InterventionParam() { Name = "Value3", TypeName = "System.String", Value = "Prefect" });

            return newSpec;
        }
    }
}
