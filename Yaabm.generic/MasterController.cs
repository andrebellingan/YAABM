using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace Yaabm.generic
{
    public abstract class MasterController<TAgent, TMultiStateModel, TLocalContext, TPopulationDynamics, TSimulation> : IDisposable
            where TAgent : Agent<TAgent>
            where TMultiStateModel : MultiStateModel<TAgent>, new()
            where TLocalContext : LocalContext<TAgent>
            where TPopulationDynamics : PopulationDynamics<TAgent>, new()
            where TSimulation : Simulation<TAgent, TMultiStateModel, TLocalContext, TPopulationDynamics, TSimulation>
    {
        private CancellationTokenSource _cancelSignal;

        private int _numberOfDays;
        private Xoshiro256StarStar _seedGenerator;

        private BlockingCollection<TSimulation> _simulations;

        public void RunAllIterations(int noOfIterations, int numberOfDays, int numberOfThreads, int seed, object modelParameters)
        {
            RunAllIterations(noOfIterations, numberOfDays, numberOfThreads, seed, numberOfThreads, modelParameters);
        }

        public void RunAllIterations(int noOfIterations, int numberOfDays, int numberOfThreads, int seed, int maxSimulationQueueSize,
            object modelParameters)
        {
            _cancelSignal = new CancellationTokenSource();
            _numberOfDays = numberOfDays;
            _seedGenerator = new Xoshiro256StarStar(seed, true);

            SaveParameters(modelParameters);

            OpenOutput();

            // this is to ensure that the memory requirements don't run away as we generate simulations faster than they can be processed
            _simulations = new BlockingCollection<TSimulation>(maxSimulationQueueSize); 

            var processingTasks = new Task[numberOfThreads + 1];
            processingTasks[0] = ProduceSimulations(noOfIterations, modelParameters, _cancelSignal.Token);

            for (var t = 0; t < numberOfThreads; t++) processingTasks[t + 1] = Task.Factory.StartNew(ConsumeSimulations, _cancelSignal.Token);

            try
            {
                Task.WaitAll(processingTasks);
                CloseOutput();
            }
            catch (AggregateException a)
            {
                InternalLog.Error(a, "Failed to run all iterations");
                throw;
            }
        }

        protected virtual void SaveParameters(object modelParameters)
        {
            // By default do nothing
        }

        protected abstract void CloseOutput();

        protected abstract void OpenOutput();

        private Task ProduceSimulations(int noOfIterations, object modelParameters, CancellationToken token)
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
                        const int milliseconds = 60 * 60 * 1000;
                        _simulations.TryAdd(newSimulation, milliseconds, token);
                    }
                    catch (OperationCanceledException  operationCanceledException)
                    {
                        InternalLog.Error(operationCanceledException, "The process to produce simulations has been cancelled");
                        _simulations.CompleteAdding();
                        break;
                    }
                    catch (Exception ex)
                    {
                        InternalLog.Error(ex, "Something went wrong generating the simulation");
                        _simulations.CompleteAdding();
                        throw;
                    }

                    InternalLog.Info($"Generated simulation {i}");
                }

                _simulations.CompleteAdding();
            }, token);
        }

        private void ConsumeSimulations()
        {
            while (!_simulations.IsCompleted)
            {
                if (!_simulations.TryTake(out var sim)) continue;

                InternalLog.Info($"Started simulation {sim.IterationNo}");

                try
                {
                    sim.Run(_numberOfDays);

                    Task.Factory.StartNew(() => AppendSimulationResultsToOutput(sim.SimulationResults));
                }
                catch (Exception ex)
                {
                    InternalLog.Error(ex, $"Something went wrong with simulation {sim.IterationNo}");
                    throw;
                }

                InternalLog.Info($"Finished simulation {sim.IterationNo}");
            }
        }

        protected abstract void AppendSimulationResultsToOutput(SimulationResults<TAgent, TMultiStateModel> itemSimulationResults);

        protected abstract TSimulation GenerateSimulation(int seed, int iterationNo, object modelParameters);

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