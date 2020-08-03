using System;
using System.IO;

namespace Yaabm.generic
{
    public interface IScenario
    {
        public int DaysToProject { get; set; }

        public DateTime StartDate { get; set; }

        string ScenarioName { get; set; }

        void SaveToFile(FileInfo scenarioFile);
    }
}
