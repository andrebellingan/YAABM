using System;
using System.Collections.Generic;
using System.Linq;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    internal class WardCounts
    {
        public WardCounts(MultiStateModel<Human> model)
        {
            foreach (var state in model.States)
            {
                StateCounts.Add(state, 0);
            }

            foreach (var transit in model.AllTransitions)
            {
                TransitionCounts.Add(transit, 0);
            }
        }

        public Dictionary<ModelState<Human>, int> StateCounts { get; } = new Dictionary<ModelState<Human>, int>();

        public Dictionary<Transition<Human>, int> TransitionCounts { get; } = new Dictionary<Transition<Human>, int>();

        public string CsvString(int wardId, in int iterationNo, in int day, in DateTime date)
        {
            var valueStrings = new List<string>
            {
                iterationNo.ToString(), day.ToString(), date.ToString("yyyy-MM-dd"), wardId.ToString()
            };

            var n = 0;
            foreach (var (_, value) in StateCounts)
            {
                n += value;
                valueStrings.Add(value.ToString());
            }
            valueStrings.Add(n.ToString());

            foreach (var transitPair in TransitionCounts)
            {
                valueStrings.Add(transitPair.Value.ToString());
            }

            return string.Join(',', valueStrings);
        }
    }

    public class CovidRecord : IDailyRecord<Human>
    {
        public CovidRecord(DateTime date, IEnumerable<Ward> wards)
        {
            Date = date;
            _wards = wards;
        }

        public DateTime Date { get; set; }

        private readonly Dictionary<int, WardCounts> _counts = new Dictionary<int, WardCounts>();

        private readonly IEnumerable<Ward> _wards;
        private MultiStateModel<Human> _model;


        public void InitializeWithStates(MultiStateModel<Human> multiStateModel)
        {
            _model = multiStateModel;
            foreach (var ward in _wards)
            {
                _counts.Add(ward.WardId, new WardCounts(multiStateModel));
            }
        }

        public void RecordTransition(Human agent, Transition<Human> transition)
        {
            var wardToUpdate = _counts[agent.WardId];
            wardToUpdate.TransitionCounts[transition]++;
        }

        public void RecordState(Human agent)
        {
            _counts[agent.WardId].StateCounts[agent.CurrentState]++;
        }

        public string CsvHeading
        {
            get
            {
                var listColumns = new List<string> {"WardId"};

                var stateNames =  _model.States.Select(state => state.Name).ToArray();
                listColumns.AddRange(stateNames);

                listColumns.Add("N");

                var transitNames = _model.AllTransitions.Select(transit => transit.Description).ToArray();
                listColumns.AddRange(transitNames);

                return string.Join(',', listColumns);
            }
        }

        public string CsvString(int iterationNo, int day)
        {
            var rowstring = new List<string>();

            foreach (var wardRecord in _counts)
            {
                rowstring.Add(wardRecord.Value.CsvString(wardRecord.Key, iterationNo, day, Date));
            }

            return string.Join('\n', rowstring);

        }
    }
}