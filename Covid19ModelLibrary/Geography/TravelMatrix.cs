using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using Serilog;
using Yaabm.generic;

namespace Covid19ModelLibrary.Geography
{
    public class TravelMatrix
    {
        private class TravelMatrixEntry
        {
            public int FromWard { get; set; }

            public int ToWard { get; set; }

            public double Probability { get; set; }
        }

        internal static TravelMatrix LoadFromCsv(string fileName)
        {
            var matrixEntries = LoadMatrixEntries(fileName);

            var travelMatrix = new TravelMatrix();

            foreach (var record in matrixEntries)
            {
                travelMatrix.AddEntry(record);
            }

            return travelMatrix;
        }

        private readonly Dictionary<int, TravelMatrixRow> _entries = new Dictionary<int, TravelMatrixRow>();

        private void AddEntry(TravelMatrixEntry entry)
        {
            if (!_entries.ContainsKey(entry.FromWard)) _entries.Add(entry.FromWard, new TravelMatrixRow());

            var fromWard = _entries[entry.FromWard];

            fromWard.AddEntry(entry.ToWard, entry.Probability);
        }

        private static IEnumerable<TravelMatrixEntry> LoadMatrixEntries(string fileName)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException($"Travel matrix file '{fileName}' does not exist");

            using var reader = new StreamReader(fileName);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = new List<TravelMatrixEntry>();

            while (csv.Read())
            {
                records.Add(csv.GetRecord<TravelMatrixEntry>());
            }

            Log.Verbose($"Loaded age distributions from {fileName}");
            return records;
        }

        public int RandomDestination(in int wardId, IRandomProvider random)
        {
            var matrixRow = _entries[wardId];
            return matrixRow.SampleDestination(random);
        }

    }
}