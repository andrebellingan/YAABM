using System;
using System.Collections.Generic;
using System.IO;
using Yaabm.generic;

namespace Covid19ModelLibrary.Population
{
    public class PopulationPyramid
    {
        public PopulationPyramid()
        {
            Provinces.Clear();
            _allProvinces = (Province[]) Enum.GetValues(typeof(Province));
            _allAgeBands = (AgeBand[]) Enum.GetValues(typeof(AgeBand));

            foreach (var province in _allProvinces)
            {
                var newProvinceList = new Dictionary<AgeBand, int>();
                Provinces.Add(province, newProvinceList);
                _provinceTotals.Add(province, 0);
                foreach (var ageBand in _allAgeBands)
                {
                    newProvinceList.Add(ageBand, 0);
                }
            }
        }

        private readonly Province[] _allProvinces;
        private readonly AgeBand[] _allAgeBands;

        public int TotalPopulation { get; private set; }

        private readonly Dictionary<Province, int> _provinceTotals = new Dictionary<Province, int>();

        private Dictionary<Province, Dictionary<AgeBand, int>> Provinces { get; } = new Dictionary<Province, Dictionary<AgeBand, int>>();

        private static readonly object FileLock = new object();

        public static PopulationPyramid LoadFromFile(string filename)
        {
            var fileInfo = new FileInfo(filename);

            //var engine = new FileHelperEngine<ProvincialAgeGroup>();

            lock (FileLock)
            {
                using var reader = fileInfo.OpenText();
                try
                {
                    return LoadFromStream(reader);
                }
                catch (Exception ex)
                {
                    InternalLog.Error(ex,$"Could not load file {filename}");
                    return null;
                }
            }
        }

        private static PopulationPyramid LoadFromStream(StreamReader reader)
        {
            var result = new PopulationPyramid();

            while (!reader.EndOfStream)
            {
                var nextLine = reader.ReadLine();
                var newRecord = ProvincialAgeGroup.FromLine(nextLine);
                result.SetNumberOfLives(newRecord.Province, newRecord.AgeBand, newRecord.NumberOfLives);
            }

            return result;
        }

        private void SetNumberOfLives(Province province, AgeBand ageBand, int value)
        {
            TotalPopulation += value;
            _provinceTotals[province]++;
            Provinces[province][ageBand] = value;
        }

        public double ScalingFactor { get; private set; }

        public void GeneratePopulation(int numberOfAgents, CovidSimulation covidSimulation)
        {
            ScalingFactor = 1d * numberOfAgents / TotalPopulation;

            foreach (var province in _allProvinces)
            {
                var livesPerProvince = 0d;
                var context = covidSimulation.GetContextByName(province.ToString());
                foreach (var ageBand in _allAgeBands)
                {
                    var numberOfLivesToCreate = Math.Round(Provinces[province][ageBand] * ScalingFactor, 0);
                    livesPerProvince += numberOfLivesToCreate;
                    for (var i = 0; i < numberOfLivesToCreate; i++)
                    {
                        var newAgent = new Human() {AgeBand = ageBand};
                        covidSimulation.AddAgent(newAgent, covidSimulation.MultiStateModel.S, context);
                    }
                }

                var hospitalSystem = context.HospitalSystem;
                hospitalSystem.SetPopulation(livesPerProvince);

            }

        }
    }
}
