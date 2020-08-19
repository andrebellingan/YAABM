using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yaabm.generic;
using Yaabm.generic.Random;

namespace UnitTests.Yaabm.generic
{
    [TestClass]
    public class TestWeightedSampler
    {
        [TestMethod]
        public void TestWeightedSampling()
        {
            var choices = new List<WeightedChoice<string>>
            {
                new WeightedChoice<string>("A", 0.25),
                new WeightedChoice<string>("B", 0.25),
                new WeightedChoice<string>("C", 0.25),
                new WeightedChoice<string>("D", 0.25)
            };

            var random = new DefaultRandom(1234);
            var result = WeightedSampler<string>.PickMultipleItems(choices, 2, random);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestReservoir()
        {
            var choices = new List<string>
            {
                "A", "B", "C", "D", "E"
            };

            var weights = new List<double>
            {
                0.3, 0.25, 0.2, 0.15, 0.1
            };

            var random = new DefaultRandom(1234);
            var selected = Reservoir.ReservoirSampling(weights, 2, random);

            Assert.AreEqual(2, selected.Count());
        }
    }
}