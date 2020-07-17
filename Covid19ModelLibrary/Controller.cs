using System;
using System.Collections.Generic;
using System.IO;
using Covid19ModelLibrary.MultiState;
using Covid19ModelLibrary.Population;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class Controller : MasterController<Human, CovidStateModel, CovidContext, CovidPopulation, CovidSimulation>
    {
        private StreamWriter _outputTextFile;
        private bool _headingHasBeenWritten;
        private readonly object _fileLock = new object();

        protected override void CloseOutput()
        {
            _outputTextFile.Close();
        }

        protected override void OpenOutput()
        {
            var fileName = SaveFilesWithDates 
                                    ? $"./Output/{ModelParameters.ScenarioName} {RunTimeStamp:yyyyMMdd} {RunTimeStamp:HHmmss} Results.csv" 
                                    : $"./Output/{ModelParameters.ScenarioName} Results.csv";

            _outputTextFile = File.CreateText(fileName);

            _headingHasBeenWritten = false;
        }

        protected override void AppendSimulationResultsToOutput(SimulationResults<Human, CovidStateModel> simulationResults)
        {
            lock (_fileLock)
            {
                if (!_headingHasBeenWritten)
                {
                    _outputTextFile.WriteLine(simulationResults.CsvHeading());
                    _headingHasBeenWritten = true;
                }

                _outputTextFile.Write(simulationResults.CsvString());
                _outputTextFile.Flush();
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
