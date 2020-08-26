using System;
using System.Collections.Generic;
using System.Linq;
using Covid19ModelLibrary.Geography;
using Covid19ModelLibrary.Initialization;
using Covid19ModelLibrary.Interventions;
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

        public CovidSimulation(int seed, int iterationNo, CovidInitializationInfo parameters, bool saveContactGraphs) : base(parameters.Scenario.StartDate, iterationNo, seed, false)
        {
            _covidInitInfo = parameters;
            _saveContactGraphs = saveContactGraphs;
            var covidScenario = (CovidScenario) parameters.Scenario;
            MultiStateModel = new CovidStateModel(covidScenario.DiseaseParameters);
            PopulationDynamics = new CovidPopulation(parameters.Scenario.NumberOfAgents);
            PopulationDynamics.Initialize(MultiStateModel);
        }

        protected override void PrepareSimulation(in int numberOfDays)
        {
            GenerateEvents(_covidInitInfo.Scenario);

            InitializeGeography(_covidInitInfo.Wards);
            InitializeHealthCareSystem(); // Need to do this after the geography has been specified
            InitializePopulation(_covidInitInfo);
            InitializeContactModel(_covidInitInfo);

            if (_saveContactGraphs) PopulationDynamics.SaveGraphs(IterationNo);
        }

        private void GenerateEvents(IScenario scenario)
        {
            var covidScenario = (CovidScenario) scenario;

            AddIntervention(new RandomlyInfectIndividuals(covidScenario.NumberExposedAtStart));
            AddIntervention(new IntroduceLockDown(5, covidScenario.DayOfLockdown5));
            AddIntervention(new IntroduceLockDown(4, covidScenario.DayOfLockDown4));
            AddIntervention(new IntroduceLockDown(3, covidScenario.DayOfLockDown3));
            AddIntervention(new IntroduceLockDown(2, covidScenario.DayOfLockDown2));
            AddIntervention(new IntroduceLockDown(0, covidScenario.DayLockDownLifted));
        }

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

        private void InitializeHealthCareSystem()
        {
            Log.Warning("Healthcare system initialization is not implemented!");
        }

        private void InitializeGeography(IList<WardRecord> wards)
        {
            AddRegions(wards, w => new RegionSpec("root", w.CountryCode, w.CountryName, "National"), GeoLevel.National);
            AddRegions(wards, w => new RegionSpec(w.CountryCode, w.ProvinceCode, w.ProvinceName, "Province"), GeoLevel.Province);
            AddRegions(wards, w => new RegionSpec(w.ProvinceCode, $"dm_{w.DistrictMunicipalityCode}", w.DistrictMunicipalityName, "DistrictMunicipality"), GeoLevel.DistrictMunicipality); // Add "dm_" prefix because of duplicate district and local municipality codes
            AddRegions(wards, w => new RegionSpec($"dm_{w.DistrictMunicipalityCode}", w.LocalMunicipalityCode, w.LocalMunicipalityName, "LocalMunicipality"), GeoLevel.LocalMunicipality);

            foreach (var ward in wards)
            {
                var newWard =
                    new Ward(ward, "Ward", PopulationDynamics, this)
                    {
                        Tag = GeoLevel.Ward
                    };
                AddLocalArea(ward.LocalMunicipalityCode, newWard);
            }
        }

        private void AddRegions(IEnumerable<WardRecord> wardRecords, Func<WardRecord, RegionSpec> regionFunc, GeoLevel level)
        {
            var regionsToAdd = wardRecords.Select(regionFunc).Distinct();

            foreach (var region in regionsToAdd)
            {
                Log.Logger.Debug($"Adding region {region.LogString()}");
                var newRegion = AddRegion(region);
                newRegion.Tag = level;
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
            return new CovidRecord(date, LocalAreas, _covidInitInfo.OutputDetail);
        }

        public void RandomlyInfect(in int numberToInfect)
        {
            var totalPopulation = PopulationDynamics.EnumeratePopulation().ToList();
            var sample = RandomProvider.RandomSelect(totalPopulation.Count, numberToInfect);
            foreach (var idx in sample)
            {
                var agentToInfect = totalPopulation[idx];
                MoveAgentToState(agentToInfect, MultiStateModel.E, RandomProvider);
            }
        }

        public void ApplyLockDownToAllWards(int lockdownLevel)
        {
            foreach (var ward in LocalAreas)
            {
                ward.LockDownLevel = lockdownLevel;
            }
        }
    }
}
