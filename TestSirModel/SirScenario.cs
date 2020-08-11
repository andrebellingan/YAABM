using System;
using System.IO;
using Yaabm.generic;

namespace TestSirModel
{
    internal class SirScenario : IScenario
    {
        public int DaysToProject { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public string ScenarioName { get; set; }
        public int NumberOfAgents { get; set; }
        public void SaveToFile(FileInfo scenarioFile)
        {
            // Not saving file
        }
    }
}