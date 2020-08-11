using System.Collections.Generic;
using System.IO;
using Covid19ModelLibrary.Initialization;
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

        protected override void PrepareOutputFiles()
        {
            _outputFileName = SaveFilesWithDates 
                                    ? $"./Output/{Scenario.ScenarioName} {RunTimeStamp:yyyyMMdd} {RunTimeStamp:HHmmss} Results.csv" 
                                    : $"./Output/{Scenario.ScenarioName} Results.csv";

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

        protected override CovidSimulation GenerateSimulation(int seed, int iterationNo, IInitializationInfo modelParameters)
        {
            var parameters = (CovidInitializationInfo) modelParameters;

            return new CovidSimulation(seed, iterationNo, parameters);
        }

        protected override IInitializationInfo PrepareInitializationInfo(IScenario scenario)
        {
            var initializationInfo = new CovidInitializationInfo();
            initializationInfo.LoadScenario(scenario);
            return initializationInfo;
        }

        internal void LoadInterventions(CovidInitializationInfo modelParameters)
        {
            var interventions = new List<IIntervention>();

            foreach (var spec in modelParameters.ModelEvents)
            {
                var newIntervention = spec.CreateInstance();
                interventions.Add(newIntervention);
            }

            base.LoadInterventions(interventions);
        }
    }
}
