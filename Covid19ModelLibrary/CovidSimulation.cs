using System;
using System.Collections.Generic;
using System.Linq;
using Covid19ModelLibrary.Geography;
using Covid19ModelLibrary.MultiState;
using Covid19ModelLibrary.Population;
using Serilog;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class CovidSimulation : Simulation<Human, CovidStateModel, Ward, CovidPopulation, CovidSimulation>
    {
        public CovidSimulation(int seed, int iterationNo, CovidModelParameters parameters) : base(parameters.StartDate, iterationNo, seed, false)
        {
            InitializeContext(parameters.WardFile, parameters);
            InitializeHealthCareSystem(parameters); // Need to do this after the geography has been specified
            InitializePopulation(parameters);
        }

        private void InitializeHealthCareSystem(CovidModelParameters parameters)
        {
            Log.Warning("Healthcare system initialization is not implemented!");
        }

        private void InitializeContext(string wardFile, CovidModelParameters parameters)
        {
            var wardRecords = WardRecord.LoadFromCsv(wardFile);

            AddRegions(wardRecords, w => new RegionSpec("root", w.CountryCode, w.CountryName, "Country"));
            AddRegions(wardRecords, w => new RegionSpec(w.CountryCode, w.ProvinceCode, w.ProvinceName, "Province"));
            AddRegions(wardRecords, w => new RegionSpec(w.ProvinceCode, $"dm_{w.DistrictMunicipalityCode}", w.DistrictMunicipalityName, "DistrictMunicipality")); // Add "dm_" prefix because of duplicate district and local municipality codes
            AddRegions(wardRecords, w => new RegionSpec($"dm_{w.DistrictMunicipalityCode}", w.LocalMunicipalityCode, w.LocalMunicipalityName, "LocalMunicipality"));

            foreach (var ward in wardRecords)
            {
                var newWard = new Ward(ward.WardId, parameters); // Get the appropriate hospital for this region
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

        private void InitializePopulation(CovidModelParameters parameters)
        {
            throw new NotImplementedException("Need to generate the population");
        }

        protected override void UpdateLocalContext(Ward asLocal)
        {
            /*
             * THIS CODE IS VERY, VERY WRONG
             *
             * For a start it can result in probabilities > 1, which is impossible
             * It is also causing the number of infections to spike hard.
             * I suspect this is mainly because the concept of a beta parameter doesn't translate to an agent based model
             */

            var currentDayResults = (CovidRecord) SimulationResults.Result(Day);

            var susceptibleTotal = currentDayResults.GetSusceptibleTotal(asLocal.Province);
            var infectiousTotal = currentDayResults.GetInfectiousTotal(asLocal.Province);

            var iAsymptomatic = PopulationDynamics.GetInfectiousBySymptoms(DiseaseSymptoms.Asymptomatic, asLocal.Province);
            var iMild = PopulationDynamics.GetInfectiousBySymptoms(DiseaseSymptoms.Mild, asLocal.Province);
            var iSevere = PopulationDynamics.GetInfectiousBySymptoms(DiseaseSymptoms.Severe, asLocal.Province);

            var infectiousStrength = asLocal.RelativeBetaAsymptomatic * iAsymptomatic + iMild + iSevere;

            var n = susceptibleTotal + infectiousTotal;
            asLocal.ProbabilityOfInfection = Math.Min(asLocal.BetaParam * asLocal.RelativeLockDownBeta * infectiousStrength / n, 1d); // probability cannot be greater than 1
        }

        protected override IDailyRecord<Human> GenerateDailyRecordInstance(int day, DateTime date)
        {
            return new CovidRecord(date);
        }
    }
}
