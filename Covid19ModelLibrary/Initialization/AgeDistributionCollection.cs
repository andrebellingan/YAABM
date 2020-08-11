using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using Serilog;

namespace Covid19ModelLibrary.Initialization
{
    public class AgeDistributionCollection : Dictionary<int, WardAgeDistribution>
    {
        private class AgeDistRecord
        {
            public int WardId { get; set; }
            public AgeBand AgeBand { get; set; }
            public double Probability { get; set; }
        }

        public static AgeDistributionCollection LoadFromCsv(string fileName)
        {
            var ageRecords = LoadAgeRecords(fileName);
            
            var ageDistributions = new AgeDistributionCollection();

            foreach (var record in ageRecords)
            {
                if (!ageDistributions.ContainsKey(record.WardId)) ageDistributions.Add(record.WardId, new WardAgeDistribution());
                var wardAges = ageDistributions[record.WardId];
                wardAges[record.AgeBand] = record.Probability;
            }

            return ageDistributions;
        }

        private static IEnumerable<AgeDistRecord> LoadAgeRecords(string fileName)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException($"Age distribution file '{fileName}' does not exist");

            using var reader = new StreamReader(fileName);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = new List<AgeDistRecord>();

            while (csv.Read())
            {
                records.Add(csv.GetRecord<AgeDistRecord>());
            }

            Log.Information($"Loaded age distributions from {fileName}");
            return records;
        }
    }
}
