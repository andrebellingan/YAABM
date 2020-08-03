using System;
using System.IO;
using TestSirModel.Model;
using Yaabm.generic;

namespace TestSirModel
{
    public class SirMasterController : MasterController<SirAgent, SirStateModel, SirContext, SirPopulationDynamics, SirSimulation>
    {
        private bool _headingHasBeenWritten;
        private readonly object _fileLock = new object();
        private string _outputFileName;
        public int SusceptibleZero { get; set; } = 9900;
        public int ExposedZero { get; set; } = 0;
        public int InfectiousZero { get; set; } = 100;
        public int ResistantZero { get; set; } = 0;

        public double RZero { get; set; } = 3.5;
        public double Gamma { get; set; } = 1d / 21d;
        public double Sigma { get; set; } = 1d / 4d;



        protected override void AppendSimulationResultsToOutput(SimulationResults<SirAgent, SirStateModel> resultSet)
        {
            lock (_fileLock)
            {
                var outputFile = File.AppendText(_outputFileName);

                if (!_headingHasBeenWritten)
                {
                    outputFile.WriteLine(resultSet.CsvHeading());
                    _headingHasBeenWritten = true;
                }

                outputFile.Write(resultSet.CsvString());
                outputFile.Close();
            }
        }

        protected override IInitializationInfo PrepareInitializationInfo(IScenario scenario)
        {
            return null;
        }

        protected override void PrepareOutputFiles()
        {
            _outputFileName = $"SEIRResults {DateTime.Today:yyyyMMdd} {DateTime.Now:HHmmsstt}.csv";
            var outputTextFile = File.CreateText(_outputFileName);
            outputTextFile.Close();
            _headingHasBeenWritten = false;
        }

        protected override SirSimulation GenerateSimulation(int seed, int iterationNo, IInitializationInfo modelParameters)
        {
            var beta = Gamma * RZero;
            var sirSim = new SirSimulation(seed, SusceptibleZero, ExposedZero, InfectiousZero, ResistantZero, beta, Gamma, Sigma, iterationNo);
            return sirSim;
        }
    }
}
