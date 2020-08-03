using System.Collections.Generic;
using Covid19ModelLibrary.Geography;
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

        private void LoadSetupFiles(CovidScenario scenario)
        {
            Wards = WardRecord.LoadFromCsv(scenario.GeographyFile);
        }
    }
}
