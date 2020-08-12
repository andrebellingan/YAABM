using System;
using System.Collections.Generic;
using System.Linq;
using Covid19ModelLibrary.Geography;
using Covid19ModelLibrary.Initialization;
using Covid19ModelLibrary.MultiState;
using Covid19ModelLibrary.Population;
using Covid19ModelLibrary.Scenarios;
using Serilog;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class CovidSimulation : Simulation<Human, CovidStateModel, Ward, CovidPopulation, CovidSimulation>
    {
        private readonly CovidInitializationInfo _covidInitInfo;
        private readonly bool _saveContactGraphs;

        public CovidSimulation(int seed, int iterationNo, CovidInitializationInfo parameters, bool saveContactGraphs) : base(parameters.Scenario.StartDate, iterationNo, seed, false, parameters.ModelEvents)
        {
            _covidInitInfo = parameters;
            _saveContactGraphs = saveContactGraphs;
            var covidScenario = (CovidScenario) parameters.Scenario;
            DiseaseParameters = covidScenario.DiseaseParameters;
        }

        protected override void PrepareSimulation(in int numberOfDays)
        {
            InitializeGeography(_covidInitInfo.Wards, _covidInitInfo);
            InitializeHealthCareSystem(_covidInitInfo); // Need to do this after the geography has been specified
            InitializePopulation(_covidInitInfo);
            InitializeContactModel(_covidInitInfo);

            if (_saveContactGraphs) PopulationDynamics.SaveGraphs(IterationNo);
        }

        public DiseaseParameters DiseaseParameters { get; }

        private void InitializeContactModel(CovidInitializationInfo parameters)
        {
            foreach (var ward in LocalAreas)
            {
                var householdDistribution = parameters.HouseHoldSizeDistributions[ward.WardId];
                ward.GenerateHouseholds(householdDistribution, parameters.HomeContactMatrix, RandomProvider);
            }

            InitializeOtherContacts(parameters);
        }

        private void InitializeOtherContacts(CovidInitializationInfo parameters)
        {
            var otherGraphGenerator = new TravelGraphGenerator();
            otherGraphGenerator.GenerateGraph(PopulationDynamics, LocalAreas, parameters.TravelMatrix, parameters.OtherContactMatrix, RandomProvider);

        }

        private void InitializeHealthCareSystem(CovidInitializationInfo parameters)
        {
            Log.Warning("Healthcare system initialization is not implemented!");
        }

        private void InitializeGeography(IList<WardRecord> wards, CovidInitializationInfo parameters)
        {
            AddRegions(wards, w => new RegionSpec("root", w.CountryCode, w.CountryName, "Country"));
            AddRegions(wards, w => new RegionSpec(w.CountryCode, w.ProvinceCode, w.ProvinceName, "Province"));
            AddRegions(wards, w => new RegionSpec(w.ProvinceCode, $"dm_{w.DistrictMunicipalityCode}", w.DistrictMunicipalityName, "DistrictMunicipality")); // Add "dm_" prefix because of duplicate district and local municipality codes
            AddRegions(wards, w => new RegionSpec($"dm_{w.DistrictMunicipalityCode}", w.LocalMunicipalityCode, w.LocalMunicipalityName, "LocalMunicipality"));

            foreach (var ward in wards)
            {
                var newWard = new Ward(ward, "Ward", PopulationDynamics, this); // Get the appropriate hospital for this region
                AddLocalArea(ward.LocalMunicipalityCode, newWard);
            }
        }

        private void AddRegions(IEnumerable<WardRecord> wardRecords, Func<WardRecord, RegionSpec> regionFunc)
        {
            var regionsToAdd = wardRecords.Select(regionFunc).Distinct();

            foreach (var region in regionsToAdd)
            {
                Log.Logger.Debug($"Adding region {region.LogString()}");
                AddRegion(region);
            }
        }

        private void InitializePopulation(CovidInitializationInfo parameters)
        {
            var correctionFactor = parameters.CalcCorrectionFactor();
            var scalingFactor = 1.0 / correctionFactor;
            foreach (var ward in LocalAreas)
            {
                var ageDistribution = parameters.AgeDistributions[ward.WardId];
                ward.GeneratePopulation(scalingFactor, ageDistribution, RandomProvider);
            }
        }



        protected override IDailyRecord<Human> GenerateDailyRecordInstance(int day, DateTime date)
        {
            return new CovidRecord(date, LocalAreas);
        }

        public void RandomlyInfect(in int numberToInfect)
        {
            var totalPopulation = PopulationDynamics.EnumeratePopulation().ToList();
            var sample = RandomProvider.RandomSelect(totalPopulation.Count, numberToInfect);
            foreach (var idx in sample)
            {
                var agentToInfect = totalPopulation[idx];
                base.MoveAgentToState(agentToInfect, MultiStateModel.I, RandomProvider);
            }
        }
    }
}
