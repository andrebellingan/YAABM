using System;
using System.Collections.Generic;
using System.Linq;
using Covid19ModelLibrary.MultiState;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    public class CovidRecord : IDailyRecord<Human>
    {
        public CovidRecord(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; set; }

        private readonly Dictionary<ModelState<Human>, int[]> _stateCounts = new Dictionary<ModelState<Human>, int[]>();

        private readonly Dictionary<Transition<Human>, int[]> _transitionCounts = new Dictionary<Transition<Human>, int[]>();

        private Province[] _allProvinces;

        private readonly Dictionary<Province, int> _susceptibleTotals = new Dictionary<Province, int>();

        private readonly Dictionary<Province, int> _infectiousTotals = new Dictionary<Province, int>();

        private ModelState<Human> _susceptibleState;

        private ModelState<Human> _deadState;

        public void InitializeWithStates(MultiStateModel<Human> multiStateModel)
        {
            _susceptibleState = ((CovidStateModel) multiStateModel).S;
            _deadState = ((CovidStateModel) multiStateModel).D;

            _allProvinces = (Province[]) Enum.GetValues(typeof(Province));

            foreach (var state in multiStateModel.States)
            {
                _stateCounts.Add(state, new int[_allProvinces.Length]);
            }

            foreach (var transition in multiStateModel.AllTransitions)
            {
                _transitionCounts.Add(transition, new int[_allProvinces.Length]);
            }

            foreach (var province in _allProvinces)
            {
                _susceptibleTotals.Add(province, 0);
                _infectiousTotals.Add(province, 0);
            }
        }

        public void RecordTransition(Human agent, Transition<Human> transition)
        {
            var provinceOffSet = (int) agent.Ward.Province;
            _transitionCounts[transition][provinceOffSet]++;
        }

        public void RecordState(Human agent)
        {
            var provinceOffset = (int) agent.Ward.Province;
            _stateCounts[agent.CurrentState][provinceOffset]++;

            if (agent.CurrentState == _susceptibleState) _susceptibleTotals[agent.Ward.Province]++;
            if (agent.IsInfectious) _infectiousTotals[agent.Ward.Province]++;
        }

        public string CsvHeading 
        {
            get
            {
                var listColumns = new List<string> {"Province"};

                var stateNames = _stateCounts.Keys.Select(s => s.Name).ToArray();
                listColumns.AddRange(stateNames);
                listColumns.Add("N");
                var transitionNames = _transitionCounts.Keys.Select(t => t.Description).ToArray();
                listColumns.AddRange(transitionNames);

                return string.Join(",", listColumns);
            }
        }

        public string CsvString(int iterationNo, int day)
        {
            var rowStrings = new List<string>();

            foreach (var province in _allProvinces)
            {
                    var values = new List<string>
                    {
                        iterationNo.ToString(),
                        day.ToString(),
                        Date.ToString("yyyy-MM-dd"),
                        province.ToString(),
                    };

                    var n = 0;
                    foreach (var stateCountPair in _stateCounts)
                    {
                        var totalCount = stateCountPair.Value[(int) province];

                        if (stateCountPair.Key != _deadState) n += totalCount;

                         values.Add(totalCount.ToString());
                    }
                    values.Add(n.ToString());

                    // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var transitionCountPair in _transitionCounts)
                    {
                        values.Add(transitionCountPair.Value[(int) province].ToString());
                    }
                    rowStrings.Add(string.Join(",", values));
            }

            return string.Join("\n", rowStrings);
        }

        public int GetSusceptibleTotal(Province province)
        {
            return _susceptibleTotals[province];
        }

        public int GetInfectiousTotal(Province province)
        {
            return _infectiousTotals[province];
        }
    }
}