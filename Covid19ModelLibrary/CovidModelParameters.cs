using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;
using Covid19ModelLibrary.Population;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    // Wrap Disease Symptom weights in a serializable wrapper

    [Serializable]
    [CollectionDataContract(Namespace = "https://YACABM/", ItemName = "Item")]
    public class SymptomWeightsList : List<WeightedChoice<DiseaseSymptoms>>
    {
    }

    [Serializable]
    [CollectionDataContract(Namespace = "https://YACABM/", ItemName = "Symptoms", KeyName = "AgeBand", ValueName = "Weights")]
    public class AgeSymptomWeights : Dictionary<AgeBand, SymptomWeightsList>
    {
        public AgeSymptomWeights()
        { }

        protected AgeSymptomWeights(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }

    // Wrap HospitalOutcomes weights in serializable wrapper

    [CollectionDataContract(Namespace = "https://YACABM/", ItemName = "Item")]
    public class HospitalOutcomeWeights : List<WeightedChoice<HospitalOutcome>>
    {
    }

    [Serializable]
    [CollectionDataContract(Namespace = "https://YACABM/", ItemName = "HospitalOutcomes", KeyName = "AgeBand", ValueName = "Weights")]
    public class AgeHospitalOutcomeWeights : Dictionary<AgeBand, HospitalOutcomeWeights>
    {
    }

    // Severe cases hospitalization weights

    [CollectionDataContract(Namespace = "https://YACABM/", ItemName = "Item")]
    public class SevereHospitalizationWeights : List<WeightedChoice<Hospitalization>>
    {
    }

    // Icu outcome weights

    [CollectionDataContract(Namespace = "https://YACABM/", ItemName = "Item")]
    public class IcuOutcomeWeights : List<WeightedChoice<IntensiveCareOutcome>>
    {
    }

    [Serializable]
    [CollectionDataContract(Namespace = "https://YACABM/", ItemName = "IcuOutcomes", KeyName = "AgeBand", ValueName = "Weights")]
    public class AgeIcuOutcomeWeights : Dictionary<AgeBand, IcuOutcomeWeights>
    {
    }

    [Serializable]
    [CollectionDataContract(Namespace = "https://YACABM/", ItemName = "Intervention")]
    public class InterventionList : List<InterventionSpec>
    {
    }

    /// <summary>
    /// Wrapper class for all the model parameters
    /// </summary>
    [DataContract(Namespace = "https://YACABM/")]
    public class CovidModelParameters
    {
        /// <summary>
        /// The name for this scenario
        /// This is used to name output files
        /// </summary>
        [DataMember(Order = 0)]
        public string ScenarioName { get; set; }

        /// <summary>
        /// The data when the simulation should start
        /// </summary>
        [DataMember(Order = 3)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The number of agents to generate for simulation
        /// </summary>
        [DataMember(Order = 2)]
        public int NumberOfAgentsToSimulate { get; set; }

        /// <summary>
        /// How many days to project into the future
        /// </summary>
        [DataMember(Order = 1)]
        public int DaysToProject { get; set; }

        /// <summary>
        /// Value of the beta parameter
        /// </summary>
        [DataMember]
        public double BetaParam { get; set; }

        /// <summary>
        /// The number of hospital beds per 1,000 lives
        /// </summary>
        [DataMember]
        public double HospitalBedsPerThousand { get; set; }

        /// <summary>
        /// The filename for the population data file used to generate the population
        /// </summary>
        [DataMember]
        public string PopulationFile { get; set; }

        /// <summary>
        /// Mean incubation period
        /// </summary>
        [DataMember]
        public double IncubationPeriod { get; set; }

        /// <summary>
        /// Average number of days a patient survives if they need a hospital bed but can't get one
        /// </summary>
        [DataMember]
        public double MeanSurvivalWaitingForHospital { get; set; }

        /// <summary>
        /// The mean number of days a person spends in a general hospital bed recovering after leaving ICY
        /// </summary>
        [DataMember]
        public double MeanDaysRecoveringAfterIcu { get; set; }

        [DataMember]
        public AgeSymptomWeights SymptomWeights { get; set; } = new AgeSymptomWeights();

        [DataMember]
        public AgeHospitalOutcomeWeights HospitalOutcomeWeights { get; set; } = new AgeHospitalOutcomeWeights();

        [DataMember]
        public SevereHospitalizationWeights SevereHospitalizationWeights { get; set; } = new SevereHospitalizationWeights();

        [DataMember] public AgeIcuOutcomeWeights IcuOutcomeWeights { get; set; } = new AgeIcuOutcomeWeights();

        [DataMember]
        public double MeanInfectiousTimeAsymptomatic { get; set; }

        [DataMember]
        public double MeanInfectiousTimeMild { get; set; }

        [DataMember]
        public double MeanInfectiousTimeSevere { get; set; }

        [DataMember]
        public double MeanTimeInIcuIfDies { get; set; }

        [DataMember]
        public double MeanTimeInIcuIfRecovers { get; set; }

        [DataMember]
        public double MeanTimeInHospitalIfIcu { get; set; }

        [DataMember]
        public double MeanTimeInHospitalIfRecover { get; set; }

        [DataMember]
        public double MeanTimeInHospitalIfDie { get; set; }

        [DataMember]
        public double SusceptibilityFactor { get; set; }

        [DataMember]
        public InterventionList Interventions { get; set; } = new InterventionList();

        [DataMember]
        public double RelativeBetaAsymptomatic { get; set; }

        public ContactMatrix ContactMatrix { get; set; }

        /// <summary>
        /// Save these model parameters to Xml
        /// </summary>
        /// <param name="fileName">The name of the XML file to save to</param>
        public void SaveToXml(string fileName)
        {
            var serializer = new DataContractSerializer(typeof(CovidModelParameters));

            var settings = new XmlWriterSettings() {Indent = true};
            using var writer = XmlWriter.Create(fileName, settings);
            serializer.WriteObject(writer, this);
        }



        public static CovidModelParameters LoadFromXml(FileInfo fileInfo)
        {
            using var fileSteam = fileInfo.Open(FileMode.Open);
            var deserializer = new DataContractSerializer(typeof(CovidModelParameters));
                
            return (CovidModelParameters)deserializer.ReadObject(fileSteam);
        }

        /// <summary>
        /// Make a deep clone of this CovidModelParameters object's data member properties
        /// </summary>
        /// <returns>A deep cloned CovidModelParameters object</returns>
        public CovidModelParameters Clone()
        {
            using var ms = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(CovidModelParameters));

            serializer.WriteObject(ms, this);
            ms.Position = 0;
            return (CovidModelParameters)serializer.ReadObject(ms);
        }

        public void SaveToJson(string fileName)
        {
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("o") // ISO 8601
            };

            var serializer = new DataContractJsonSerializer(typeof(CovidModelParameters), settings);

            using var stream = File.Create(fileName);
            serializer.WriteObject(stream, this);
        }

        public static CovidModelParameters LoadFromJson(FileInfo fileInfo)
        {
            using var fileStream = fileInfo.Open(FileMode.Open);
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("o")
            };

            var deserializer = new DataContractJsonSerializer(typeof(CovidModelParameters), settings);

            return (CovidModelParameters) deserializer.ReadObject(fileStream);
        }

        #region Paramters are files

        [DataMember]
        public string WardFile { get; set; }


        #endregion
    }
}
