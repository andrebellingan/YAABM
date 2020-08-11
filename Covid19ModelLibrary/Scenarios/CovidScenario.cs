using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Serilog;
using Yaabm.generic;

namespace Covid19ModelLibrary.Scenarios
{
    [DataContract(Namespace = "https://YACABM/")]
    public class CovidScenario : IScenario
    {
        [DataMember]
        public string ScenarioName { get; set; }

        public void SaveToFile(FileInfo scenarioFile)
        {
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("o") // ISO 8601
            };

            var serializer = new DataContractJsonSerializer(typeof(CovidScenario), settings);

            using var stream = scenarioFile.Create();
            serializer.WriteObject(stream, this);
        }

        public static CovidScenario LoadFromFile(FileInfo file)
        {
            if (!file.Exists)
            {
                Log.Error($"The scenario file {file.FullName} does not exist.");
                throw new FileNotFoundException("Cannot find scenario file", file.FullName);
            }

            using var fileStream = file.Open(FileMode.Open);
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("o")
            };

            var deserializer = new DataContractJsonSerializer(typeof(CovidScenario), settings);

            return (CovidScenario)deserializer.ReadObject(fileStream);
        }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public int DaysToProject { get; set; }

        [DataMember]
        public int NumberOfAgents { get; set; }

        [DataMember]
        public string GeographyFile { get; set; }

        [DataMember]
        public string AgeDistributionsFile { get; set; }

        [DataMember]
        public string HouseHoldSizesFile { get; set; }

        [DataMember]
        public string HomeContactMatrixFile { get; set; }

        [DataMember]
        public string OtherContactMatrixFile { get; set; }

        [DataMember]
        public string DiseaseParametersFile { get; set; }

        [DataMember]
        public string ModelEventsFile { get; set; }

        [DataMember]
        public string TravelMatrixFile { get; set; }
    }
}
