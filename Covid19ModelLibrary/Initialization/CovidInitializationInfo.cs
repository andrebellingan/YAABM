using System;
using System.Collections.Generic;
using System.Linq;
using Covid19ModelLibrary.Geography;
using Covid19ModelLibrary.Population;
using Yaabm.generic;

namespace Covid19ModelLibrary.Initialization
{
    public class CovidInitializationInfo : IInitializationInfo
    {
        public IScenario Scenario { get; private set; }

        public void LoadScenario(IScenario scenario)
        {
            var covidScenario = scenario as CovidScenario;

            Scenario = scenario;
            LoadSetupFiles(covidScenario);
        }

        public List<InterventionSpec> ModelEvents { get; }

        public IList<WardRecord> Wards { get; private set; }

        public AgeDistributionCollection AgeDistributions { get; private set; }

        public HouseSizeDistributionCollection HouseHoldSizeDistributions { get; private set; }
        public ContactMatrix HomeContactMatrix { get; private set; }

        private void LoadSetupFiles(CovidScenario scenario)
        {
            Wards = WardRecord.LoadFromCsv(scenario.GeographyFile);
            AgeDistributions = AgeDistributionCollection.LoadFromCsv(scenario.AgeDistributionsFile);
            HouseHoldSizeDistributions = HouseSizeDistributionCollection.LoadFromCsv(scenario.HouseHoldSizesFile);
            HomeContactMatrix = ContactMatrix.LoadFromCsv(scenario.HomeContactMatrixFile);
        }

        public double CalcCorrectionFactor()
        {
            var totalPopulation = Wards.Sum(w => w.Population);
            return totalPopulation / Convert.ToDouble(Scenario.NumberOfAgents);
        }
    }
}
