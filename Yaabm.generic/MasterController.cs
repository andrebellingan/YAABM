using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Random;
using Serilog;

namespace Yaabm.generic
{
    public abstract class MasterController<TAgent, TMultiStateModel, TLocalContext, TPopulationDynamics, TSimulation> : IDisposable
            where TAgent : Agent<TAgent>
            where TMultiStateModel : MultiStateModel<TAgent>, new()
            where TLocalContext : LocalArea<TAgent>
            where TPopulationDynamics : PopulationDynamics<TAgent>, new()
            where TSimulation : Simulation<TAgent, TMultiStateModel, TLocalContext, TPopulationDynamics, TSimulation>
    {
        private CancellationTokenSource _cancelSignal;

        private int _numberOfDays;
        private Xoshiro256StarStar _seedGenerator;

        private BlockingCollection<TSimulation> _simulations;

        public void RunAllIterations(IScenario scenario, int noOfIterations, int numberOfThreads, int seed,
            int maxSimulationQueueSize, bool addDatesToOutputFileNames = false)
        {
            SaveFilesWithDates = addDatesToOutputFileNames;

            _cancelSignal = new CancellationTokenSource();
            _numberOfDays = scenario.DaysToProject;
            _seedGenerator = new Xoshiro256StarStar(seed, true);

            var initializationInfo = PrepareInitializationInfo(scenario);
            SaveScenario(scenario);
            OpenOutput();

            // this is to ensure that the memory requirements don't run away as we generate simulations faster than they can be processed
            _simulations = new BlockingCollection<TSimulation>(maxSimulationQueueSize); 

            var processingTasks = new Task[numberOfThreads + 1];
            processingTasks[0] = ProduceSimulations(noOfIterations, initializationInfo, _cancelSignal.Token);

            for (var t = 0; t < numberOfThreads; t++) processingTasks[t + 1] = Task.Factory.StartNew(ConsumeSimulations, _cancelSignal.Token);

            try
            {
                Task.WaitAll(processingTasks);
                Log.Information("All processing threads have finished");
            }
            catch (AggregateException a)
            {
                Log.Error(a, "Failed to run all iterations");
                throw;
            }
        }

        public DateTime RunTimeStamp { get; private set; }

        public bool SaveFilesWithDates { get; set; }

        private void SaveScenario(IScenario scenario)
        {
            Scenario = scenario;

            RunTimeStamp = DateTime.Now;

            if (!Directory.Exists("./Output")) Directory.CreateDirectory("./Output");

            var fileName = SaveFilesWithDates
                ? $"./Output/{scenario.ScenarioName} {RunTimeStamp:yyyyMMdd HHmmss} parameters.json"
                : $"./Output/{scenario.ScenarioName} parameters.json";

            var file = new FileInfo(fileName);

            scenario.SaveToFile(file);
        }

        public IScenario Scenario { get; private set; }

        protected abstract IInitializationInfo PrepareInitializationInfo(IScenario scenario);

        protected abstract void OpenOutput();

        private Task ProduceSimulations(int noOfIterations, IInitializationInfo modelParameters, CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < noOfIterations; i++)
                {
                    var simulationSeed = _seedGenerator.Next();

                    try
                    {
                        var newSimulation = GenerateSimulation(simulationSeed, i, modelParameters);
                        newSimulation.AddInterventions(_interventions);
                        const int milliseconds = 60 * 60 * 1000; //TODO: Hardcoded value for cancellation timeout
                        _simulations.TryAdd(newSimulation, milliseconds, token);
                    }
                    catch (OperationCanceledException operationCanceledException)
                    {
                        Log.Error(operationCanceledException, "The process to produce simulations has been cancelled");
                        _simulations.CompleteAdding();
                        break;
                    }
                    catch (NotImplementedException f)
                    {
                        Log.Error(f,"Failed while generating simulation: A method is not implemented!", f.TargetSite);
                        _simulations.CompleteAdding();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Something went wrong generating the simulation");
                        _simulations.CompleteAdding();
                        throw;
                    }

                    Log.Information($"Generated simulation {i}");
                }

                _simulations.CompleteAdding();
            }, token);
        }

        private void ConsumeSimulations()
        {
            while (!_simulations.IsCompleted)
            {
                if (!_simulations.TryTake(out var sim)) continue;

                Log.Information($"Started simulation {sim.IterationNo}");

                try
                {
                    sim.Run(_numberOfDays);

                    Task.Factory.StartNew(() => AppendSimulationResultsToOutput(sim.SimulationResults));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Something went wrong with simulation {sim.IterationNo}");
                    throw;
                }

                Log.Information($"Finished simulation {sim.IterationNo}");
            }
        }

        protected abstract void AppendSimulationResultsToOutput(SimulationResults<TAgent, TMultiStateModel> itemSimulationResults);

        protected abstract TSimulation GenerateSimulation(int seed, int iterationNo, IInitializationInfo modelParameters);

        protected IInitializationInfo InitializationInfo { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _cancelSignal?.Dispose();
                _simulations?.Dispose();
            }

            _disposed = true;
        }

        private readonly List<IIntervention> _interventions = new List<IIntervention>();

        public void LoadInterventions(IEnumerable<IIntervention> interventions)
        {
            _interventions.AddRange(interventions);
        }
    }
}