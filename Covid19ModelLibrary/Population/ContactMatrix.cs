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

        // ReSharper disable once ClassNeverInstantiated.Local
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

            Log.Verbose($"Loaded contact matrix from {fileName}");
            return records;
        }

        public Dictionary<AgeBand, double> Contacts(AgeBand ageBand)
        {
            return _contactValues[ageBand];
        }

        public IEnumerable<double> ContactWeightedAgentList(List<int> potentialContacts, CovidPopulation environment, Human agent)
        {
            var avgContacts = Contacts(agent.AgeBand);

            var totalWeight = 0d;
            foreach (var otherContactId in potentialContacts)
            {
                var otherContact = environment.AgentById(otherContactId);
                var weight = avgContacts[otherContact.AgeBand];
                totalWeight += weight;
            }

            foreach (var otherContactId in potentialContacts)
            {
                var otherContact = environment.AgentById(otherContactId);
                var weight = avgContacts[otherContact.AgeBand];
                yield return weight / totalWeight;
            }
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