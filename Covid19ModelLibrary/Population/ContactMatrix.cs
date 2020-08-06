using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Serilog;
using Yaabm.generic;

namespace Covid19ModelLibrary.Population
{
    public class ContactMatrix
    {
        private readonly Dictionary<AgeBand, Dictionary<AgeBand, double>> _contactValues = new Dictionary<AgeBand, Dictionary<AgeBand, double>>();

        public ContactMatrix()
        {
            var allAgeBands = (AgeBand[]) Enum.GetValues(typeof(AgeBand));
            foreach (var ageBand in allAgeBands)
            {
                var newAgeBand = allAgeBands.ToDictionary<AgeBand, AgeBand, double>(otherAgeBand => otherAgeBand, otherAgeBand => 0);
                _contactValues.Add(ageBand, newAgeBand);
            }
        }

        private class ContactRecord
        {
            public AgeBand AgeBand { get; set; }
            public AgeBand OtherAgeBand { get; set; }
            public double Contacts { get; set; }
        }

        public static ContactMatrix LoadFromCsv(string fileName)
        {
            var contactRecords = LoadContactRecords(fileName);

            var matrix = new ContactMatrix();

            foreach (var record in contactRecords)
            {
                matrix.SetContactValue(record.AgeBand, record.OtherAgeBand, record.Contacts);
            }

            return matrix;
        }

        private void SetContactValue(AgeBand ageBand, AgeBand otherAgeBand, in double contacts)
        {
            _contactValues[ageBand][otherAgeBand] = contacts;
        }

        private static IEnumerable<ContactRecord> LoadContactRecords(string fileName)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException($"Contact matrix file '{fileName}' does not exist");

            using var reader = new StreamReader(fileName);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = new List<ContactRecord>();

            while (csv.Read())
            {
                records.Add(csv.GetRecord<ContactRecord>());
            }

            Log.Information($"Loaded contact matrix from {fileName}");
            return records;
        }

        public Dictionary<AgeBand, double> Contacts(AgeBand ageBand)
        {
            return _contactValues[ageBand];
        }

        public List<Tuple<Human, double>> ContactWeightedAgentList(IEnumerable<Human> potentialContacts, Human agent)
        {
            var weights = new List<Tuple<Human, double>>();

            var contracts = Contacts(agent.AgeBand);

            var totalWeight = 0d;
            foreach (var other in potentialContacts)
            {
                var weight = contracts[other.AgeBand];
                totalWeight += weight;
                weights.Add(new Tuple<Human, double>(other, weight));
            }

            var result = new List<Tuple<Human, double>>();

            foreach (var t in weights)
            {
                result.Add(new Tuple<Human, double>(t.Item1, t.Item2 / totalWeight));
            }

            return result;
        }

        public int SampleNoOfContacts(Human agent, IRandomProvider random)
        {
            var contractRow = Contacts(agent.AgeBand);

            var totalContacts = 0;
            foreach (var item in contractRow)
            {
                totalContacts += random.SamplePoisson(item.Value);
            }

            return totalContacts;
        }
    }
}