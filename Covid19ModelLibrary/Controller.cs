using System.IO;
using Covid19ModelLibrary.Geography;
using Covid19ModelLibrary.Initialization;
using Covid19ModelLibrary.MultiState;
using Covid19ModelLibrary.Population;
using Serilog;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class Controller : MasterController<Human, CovidStateModel, Ward, CovidPopulation, CovidSimulation>
    {
        private string _outputFileName;
        private bool _headingHasBeenWritten;
        private readonly object _fileLock = new object();
        public bool SaveContactGraphs { get; set; }
        public GeoLevel OutputDetail { get; set; } = GeoLevel.National;

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
                Log.Verbose($"Saving results for Simulation {simulationResults.IterationNumber}");
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
            var parameters = (CovidInitializationInfo) PrepareInitializationInfo(Scenario);

            return new CovidSimulation(seed, iterationNo, parameters, SaveContactGraphs);
        }

        protected override IInitializationInfo PrepareInitializationInfo(IScenario scenario)
        {
            var initializationInfo = new CovidInitializationInfo();
            initializationInfo.LoadScenario(scenario);
            initializationInfo.OutputDetail = OutputDetail;
            return initializationInfo;
        }
    }
}
