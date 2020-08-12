using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Serilog;

namespace Covid19ModelLibrary.Initialization
{
    public class HouseSizeDistributionCollection : Dictionary<int, WardHouseSizeDistribution>
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private class HhSizeRecord
        {
            public int WardId { get; set; }
            public int Size { get; set; }
            public double NoOfHouseholds { get; set; }
        }

        public static HouseSizeDistributionCollection LoadFromCsv(string fileName)
        {
            var sizeRecords = LoadSizeRecords(fileName);
            var maxHouseHoldSize = sizeRecords.Max(r => r.Size);

            var householdSizes = new HouseSizeDistributionCollection();

            foreach (var sizeRec in sizeRecords)
            {
                if (!householdSizes.ContainsKey(sizeRec.WardId)) householdSizes.Add(sizeRec.WardId, new WardHouseSizeDistribution(maxHouseHoldSize));
                var wardHouseSizeDist = householdSizes[sizeRec.WardId];
                wardHouseSizeDist[sizeRec.Size] = sizeRec.NoOfHouseholds;
            }

            Log.Verbose($"Loaded household sizes from {fileName}");

            return householdSizes;
        }

        private static List<HhSizeRecord> LoadSizeRecords(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException($"Household size distribution file '{fileName}' does not exist");

            using var reader = new StreamReader(fileName);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = new List<HhSizeRecord>();

            while (csv.Read())
            {
                records.Add(csv.GetRecord<HhSizeRecord>());
            }

            Log.Verbose($"Loaded age distributions from {fileName}");
            return records;
        }
    }
}