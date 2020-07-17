using Yaabm.generic;

namespace Covid19ModelLibrary.Interventions
{
    public class ImportCasesToProvince : IIntervention
    {
        private readonly int _v;

        private readonly AgeBand _ageBand;

        private readonly Province _province;

        public ImportCasesToProvince(int numberOfImports, AgeBand ageBand, Province province)
        {
            _v = numberOfImports;
            _ageBand = ageBand;
            _province = province;
        }

        public int DayOfIntervention { get; set; }

        public string Description => $"Import {_v} cases";

        public void Apply(object targetSimulation)
        {
            var simulation = (CovidSimulation) targetSimulation;

             for (var i = 0; i < _v; i++)
            {
                var newDude = new Human {AgeBand = _ageBand};
                var newContext = simulation.GetContextByName(_province.ToString());
                var newState = simulation.MultiStateModel.E;
                simulation.AddAgent(newDude, newState, newContext);
            }
        }
    }
}