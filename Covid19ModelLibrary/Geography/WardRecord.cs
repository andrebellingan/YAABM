using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using Serilog;

namespace Covid19ModelLibrary.Geography
{
    internal class WardRecord
    {
        public int ObjectId { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public string ProvinceCode { get; set; }

        public string ProvinceName { get; set; }

        public string LocalMunicipalityCode { get; set; }

        public int WardNumber { get; set; }

        public int WardId { get; set; }

        public string LocalMunicipalityName { get; set; }

        public string DistrictMunicipalityCode { get; set; }

        public string DistrictMunicipalityName { get; set; }

        public int Year { get; set; }

        public double ShapeArea { get; set; }

        public double ShapeLength { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public static IList<WardRecord> LoadFromCsv(string filename)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException($"Ward file '{filename}' does not exist");

            using var reader = new StreamReader(filename);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var result = new List<WardRecord>();

            while (csv.Read())
            {
                result.Add(csv.GetRecord<WardRecord>());
            }

            Log.Information($"Loaded geographic information from {filename}");
            return result;
        }
    }
}
