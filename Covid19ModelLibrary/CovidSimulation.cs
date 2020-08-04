using System;
using System.Collections.Generic;
using System.Linq;
using Covid19ModelLibrary.Geography;
using Covid19ModelLibrary.Initialization;
using Covid19ModelLibrary.MultiState;
using Covid19ModelLibrary.Population;
using Serilog;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class CovidSimulation : Simulation<Human, CovidStateModel, Ward, CovidPopulation, CovidSimulation>
    {
        public CovidSimulation(int seed, int iterationNo, CovidInitializationInfo parameters) : base(parameters.Scenario.StartDate, iterationNo, seed, false)
        {
            InitializeGeography(parameters.Wards, parameters);
            InitializeHealthCareSystem(parameters); // Need to do this after the geography has been specified
            InitializePopulation(parameters);
            InitializeContactModel(parameters);
            PopulationDynamics.SaveGraphs(IterationNo);
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
           Log.Error("CITY-WIDE CONTACT GRAPH IS NOT IMPLEMENTED!");
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
                var newWard = new Ward(ward, "Ward", PopulationDynamics); // Get the appropriate hospital for this region
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

        protected override void UpdateLocalContext(Ward asLocal)
        {
            Log.Warning("CovidSimulation.UpdateLocalContext() is not implemented!");
        }

        protected override IDailyRecord<Human> GenerateDailyRecordInstance(int day, DateTime date)
        {
            return new CovidRecord(date);
        }
    }
}
