using System.IO;
using CommandLineParser.Arguments;

namespace RunCovidSimulation
{
    internal class RunSettings
    {
        [ValueArgument(
            typeof(int), 
            'n', 
            "iterations", 
            AllowMultiple = false, 
            Description = "How many Monte-Carlo iterations to simulate",
            Optional = false,
            ValueOptional = false
        )]
        public int Iterations { get; set; }

        [FileArgument(
            's',
            "scenario",
            Aliases = new string[]{},
            AllowMultiple = false,
            Description = "Name of the scenario file to run",
            Example = "--scenario ./Scenarios/Baseline_scenario.json",
            FileMustExist = true,
            Optional = false
        )]
        public FileInfo ScenarioFile { get; set; }

        [ValueArgument(
            typeof(int),
            't',
            "threads",
            Aliases = new string[] {},
            DefaultValue = 1,
            AllowMultiple = false,
            Description = "Number of processing threads to use",
            Optional = true,
            ValueOptional = false
            )]
        public int Threads { get; set; }

        [SwitchArgument('d', "saveDate", false, Description = "Add the date and time to the output file names", Optional = true)]
        public bool SaveDate { get; set; }
        
        [SwitchArgument('c', "contactGraphs", false, Description = "Save the contact graph for each iteration to GraphML files", Optional = true)]
        public bool SaveContactGraphs { get; set; }

        [EnumeratedValueArgument(typeof(string), 'g', "geoDetail", AllowedValues = "National;Province;DistrictMunicipality;LocalMunicipality;Ward", Description = "Level of geographical detail in the output.", Optional = true, AllowMultiple = false, DefaultValue = "National")]
        public string OutputDetail { get; set; }
    }
}