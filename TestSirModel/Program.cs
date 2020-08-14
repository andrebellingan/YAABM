using System;
using Serilog;
using Serilog.Events;

namespace TestSirModel
{
    internal class Program
    {
        private static void Main()
        {
            SetupLogging();

            const int iterations = 1000;
            const int noOfDays = 100;
            const int seed = 879375928;
            const int noOfThreads = 10;
            const int maxQueueSize = 3;

            var controller = new SirMasterController
            {
                SusceptibleZero = 99900,
                ExposedZero = 80,
                InfectiousZero = 10,
                ResistantZero = 10,
                RZero = 3.5,
                Gamma = 1d / 21d,
                Sigma = 1d / 4d
            };

            var startTime = DateTime.Now;

            var scenario = new SirScenario
            {
                DaysToProject = noOfDays, 
                ScenarioName = "SEIR_Test"
            };


            controller.RunAllIterations(scenario, iterations, noOfThreads, seed, maxQueueSize);

            var endTime = DateTime.Now;
            var timePassed = endTime - startTime;

            Console.WriteLine($"Time taken: {timePassed:g}");
        }

        private static void SetupLogging()
        {
            var logFileName = $"{DateTime.Now:yyyyMMdd hh-mm-ss}.log";

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Verbose()
#elif DEBUG
                .MinimumLevel.Debug()
#endif
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
#if DEBUG
                .WriteTo.File($"./logs/{logFileName}", restrictedToMinimumLevel: LogEventLevel.Verbose)
#elif DEBUG
                .WriteTo.File($"./logs/{logFileName}", restrictedToMinimumLevel: LogEventLevel.Debug)
#endif
                .CreateLogger();
        }

    }
}
