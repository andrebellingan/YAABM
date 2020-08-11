using Yaabm.generic;

namespace Covid19ModelLibrary.Interventions
{
    public class ImportCasesToProvince : IIntervention
    {
        private readonly int _v;

        private readonly AgeBand _ageBand;
        private string _wardId;

        public ImportCasesToProvince(int numberOfImports, AgeBand ageBand, string wardId)
        {
            _v = numberOfImports;
            _ageBand = ageBand;
            _wardId = wardId;
        }

        public int DayOfIntervention { get; set; }

        public string Description => $"Import {_v} cases";

        public void Apply(object targetSimulation)
        {
            var simulation = (CovidSimulation) targetSimulation;

             for (var i = 0; i < _v; i++)
            {
                var newState = simulation.MultiStateModel.E;
                var newContext = simulation.GetContextByName(_wardId);
                var newDude = simulation.AddAgent(newState, newContext);
                newDude.AgeBand = _ageBand;
            }
        }
    }
}