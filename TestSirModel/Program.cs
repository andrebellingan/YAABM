using System;

namespace TestSirModel
{
    internal class Program
    {
        private static void Main()
        {
            const int iterations = 100;
            const int noOfDays = 365;
            const int seed = 879375928;
            const int noOfThreads = 11;

            var controller = new SirMasterController
            {
                SusceptibleZero = 9990,
                ExposedZero = 9,
                InfectiousZero = 1,
                ResistantZero = 0,
                RZero = 3.0,
                Gamma = 1d / 21d,
                Sigma = 1d / 4d
            };

            var startTime = DateTime.Now;

            controller.RunAllIterations(iterations, noOfDays, noOfThreads, seed, null);

            var endTime = DateTime.Now;
            var timePassed = endTime - startTime;

            Console.WriteLine($"Time take: {timePassed:g}");

            Console.WriteLine("Press any key...");

            Console.ReadKey();
        }

    }
}
