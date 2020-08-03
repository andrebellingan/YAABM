using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLineParser.Exceptions;
using Covid19ModelLibrary;
using Covid19ModelLibrary.Initialization;
using Serilog;
using Serilog.Events;

namespace RunCovidSimulation
{
    public class Program
    {
        private static int Main(string[] args)
        {
            SetupLogging();

            if (!CheckArgumentParserVersion()) return -1;

            var runSettings = new RunSettings();

            if (!ParseArguments(args, runSettings))
            {
                Log.Error("Execution stopped because command line parameters did not parse successfully.");
                return -1;
            }

            var iterations = runSettings.Iterations;

            const int seed = 879375928;

            var noOfThreads = Environment.ProcessorCount; // by default use everything that is available
            if (runSettings.Threads != 0)
            {
                noOfThreads = runSettings.Threads;
            }
            Log.Information($"Processing on {noOfThreads} threads");

            var maximumQueueSize = Math.Min(noOfThreads, 3);

            var controller = new Controller {SaveFilesWithDates = runSettings.SaveDate};

            var startTime = DateTime.Now;

            var scenario = CovidScenario.LoadFromFile(runSettings.ScenarioFile);

            controller.RunAllIterations(scenario, iterations, noOfThreads, seed, maximumQueueSize);

            var endTime = DateTime.Now;
            var timePassed = endTime - startTime;

            Log.Information($"Total processing time: {timePassed:g}");

            Log.CloseAndFlush();
            return 0;
        }

        private static void SetupLogging()
        {
            var logFileName = $"{DateTime.Today:yyyy-MM-dd}.log";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(restrictedToMinimumLevel:LogEventLevel.Information)
                .WriteTo.File($"./logs/{logFileName}", restrictedToMinimumLevel: LogEventLevel.Debug)
                .CreateLogger();
        }

        private static bool CheckArgumentParserVersion()
        {
            var parserAssembly = Assembly.GetAssembly(typeof(CommandLineParser.CommandLineParser));

            if (parserAssembly == null) return false;

            var version = parserAssembly.GetName().Version;
            if (version == null) return false;
            if (version.Major == 3 && version.Minor == 0 && version.MajorRevision == 20)
            {
                Console.WriteLine($"Due to a bug in the package {parserAssembly.FullName} version 3.0.20 the command line parser may not work correctly. Please install 3.0.19 or a version later than 3.0.20 using nuget");
                return false;
            }

            Console.WriteLine($"Using command line arguments parser version {version}");

            return true;
        }

        private static bool ParseArguments(string[] args, object targetObject)
        {
            var parser = new CommandLineParser.CommandLineParser()
            {
                ShowUsageOnEmptyCommandline = true,
                AcceptEqualSignSyntaxForValueArguments = true,
                AcceptHyphen = true,
                AcceptSlash = false,
                AllowShortSwitchGrouping = false,
                CheckMandatoryArguments = true
            };

            parser.ExtractArgumentAttributes(targetObject);
            if (args.Any(arg => arg.ToLower() == "--help"))
            {
                parser.PrintUsage(Console.Out);
                return false;
            }

            try
            {
                parser.ParseCommandLine(args);
#if DEBUG
                parser.ShowParsedArguments();
#endif
            }
            catch (MandatoryArgumentNotSetException m)
            {
                Log.Error(m, "Command line parsing error");
                Console.WriteLine($"A mandatory argument {m.Argument} was not set");
                parser.PrintUsage(Console.Out);
                return false;
            }
            catch (FileNotFoundException f)
            {
                Log.Error(f, "Specified file does not exist", f.FileName);
                return false;
            }
            catch (CommandLineException e)
            {
                Log.Error(e, "Command line exception");
                return false;
            }

            return parser.ParsingSucceeded;
        }
    }
}
