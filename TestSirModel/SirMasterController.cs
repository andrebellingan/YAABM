using System;
using System.IO;
using TestSirModel.Model;
using Yaabm.generic;

namespace TestSirModel
{
    public class SirMasterController : MasterController<SirAgent, SirStateModel, SirContext, SirPopulationDynamics, SirSimulation>
    {
        private StreamWriter _outputTextFile;
        private bool _headingHasBeenWritten;
        private readonly object _fileLock = new object();
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
                if (!_headingHasBeenWritten)
                {
                    _outputTextFile.WriteLine(resultSet.CsvHeading());
                    _headingHasBeenWritten = true;
                }

                _outputTextFile.Write(resultSet.CsvString());
                _outputTextFile.Flush();
            }
        }

        protected override void OpenOutput()
        {
            var fileName = $"SEIRResults {DateTime.Today:yyyyMMdd} {DateTime.Now:HHmmsstt}.csv";
            _outputTextFile = File.CreateText(fileName);
            _headingHasBeenWritten = false;
        }

        protected override void CloseOutput()
        {
            _outputTextFile.Close();
        }



 

        protected override SirSimulation GenerateSimulation(int seed, int iterationNo, object modelParameters)
        {
            var beta = Gamma * RZero;
            var sirSim = new SirSimulation(seed, SusceptibleZero, ExposedZero, InfectiousZero, ResistantZero, beta, Gamma, Sigma, iterationNo);
            return sirSim;
        }
    }
}
