using System;
using System.Collections.Generic;
using System.IO;
using Covid19ModelLibrary.MultiState;
using Covid19ModelLibrary.Population;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class Controller : MasterController<Human, CovidStateModel, Ward, CovidPopulation, CovidSimulation>
    {
        private string _outputFileName;
        private bool _headingHasBeenWritten;
        private readonly object _fileLock = new object();

        protected override void OpenOutput()
        {
            _outputFileName = SaveFilesWithDates 
                                    ? $"./Output/{ModelParameters.ScenarioName} {RunTimeStamp:yyyyMMdd} {RunTimeStamp:HHmmss} Results.csv" 
                                    : $"./Output/{ModelParameters.ScenarioName} Results.csv";

            var outputTextFile = File.CreateText(_outputFileName);
            outputTextFile.Close();

            _headingHasBeenWritten = false;
        }

        protected override void AppendSimulationResultsToOutput(SimulationResults<Human, CovidStateModel> simulationResults)
        {
            lock (_fileLock)
            {
                var outputFile = File.AppendText(_outputFileName);

                if (!_headingHasBeenWritten)
                {
                    outputFile.WriteLine(simulationResults.CsvHeading());
                    _headingHasBeenWritten = true;
                }

                outputFile.Write(simulationResults.CsvString());
                outputFile.Close();
            }
        }

        protected override CovidSimulation GenerateSimulation(int seed, int iterationNo, object modelParameters)
        {
            var parameters = (CovidModelParameters) modelParameters;

            return new CovidSimulation(seed, iterationNo, parameters);
        }

        protected override void SaveParameters(object modelParameters)
        {
            RunTimeStamp = DateTime.Now;

            ModelParameters = (CovidModelParameters) modelParameters;

            if (!Directory.Exists("./Output")) Directory.CreateDirectory("./Output");

            ModelParameters.SaveToJson(
                SaveFilesWithDates
                    ? $"./Output/{ModelParameters.ScenarioName} {RunTimeStamp:yyyyMMdd HHmmss} parameters.json"
                    : $"./Output/{ModelParameters.ScenarioName} parameters.json");
        }

        public DateTime RunTimeStamp { get; private set; }

        public CovidModelParameters ModelParameters { get; set; }

        public bool SaveFilesWithDates { get; set; }

        public void LoadInterventions(InterventionList interventionSpecs)
        {
            var interventions = new List<IIntervention>();

            foreach (var spec in interventionSpecs)
            {
                var newIntervention = spec.CreateInstance();
                interventions.Add(newIntervention);
            }

            base.LoadInterventions(interventions);
        }
    }
}
