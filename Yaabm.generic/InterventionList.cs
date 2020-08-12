using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Serilog;

namespace Yaabm.generic
{
    [Serializable]
    [CollectionDataContract(Namespace = "https://YACABM/", ItemName = "Intervention")]
    public class InterventionList : List<InterventionSpec>
    {
        public static InterventionList LoadFromFile(string fileName)
        {
            var file = new FileInfo(fileName);
            return LoadFromFile(file);
        }

        public static InterventionList LoadFromFile(FileInfo file)
        {
            if (!file.Exists)
            {
                Log.Error($"The events file {file.FullName} does not exist.");
                throw new FileNotFoundException("Cannot find intervention file", file.FullName);
            }

            using var fileStream = file.Open(FileMode.Open);
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("o")
            };

            var deserializer = new DataContractJsonSerializer(typeof(InterventionList), settings);

            Log.Verbose($"Loaded model events from {file.Name}");

            return (InterventionList)deserializer.ReadObject(fileStream);
        }

        public void SaveToFile(FileInfo file)
        {
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("o") // ISO 8601
            };

            var serializer = new DataContractJsonSerializer(typeof(InterventionList), settings);

            using var stream = file.Create();
            serializer.WriteObject(stream, this);
        }
    }
}
