using System;
using System.Collections.Generic;
using System.Linq;
using Covid19ModelLibrary.Geography;
using Yaabm.generic;

namespace Covid19ModelLibrary
{
    internal class RegionCounts
    {
        public RegionCounts(MultiStateModel<Human> model)
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

        public string CsvString(string areaName, in int iterationNo, in int day, in DateTime date)
        {
            var valueStrings = new List<string>
            {
                iterationNo.ToString(), day.ToString(), date.ToString("yyyy-MM-dd"), areaName
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
        public CovidRecord(DateTime date, IEnumerable<Ward> wards, GeoLevel outputLevel)
        {
            Date = date;
            OutputLevel = outputLevel;
            _wards = wards;

            PrepareRegions();
        }

        private void PrepareRegions()
        {
            _regions = new List<GeographicArea<Human>>();
            _wardMap = new Dictionary<Ward, GeographicArea<Human>>();
            foreach (var ward in _wards)
            {
                var region = ward.AreaAtLevel(OutputLevel);
                if (!_regions.Contains(region)) _regions.Add(region);
                _wardMap.Add(ward, region);
            }
        }

        public GeoLevel OutputLevel { get; set; }

        public DateTime Date { get; set; }

        private readonly Dictionary<GeographicArea<Human>, RegionCounts> _counts = new Dictionary<GeographicArea<Human>, RegionCounts>();

        private List<GeographicArea<Human>> _regions;
        private Dictionary<Ward, GeographicArea<Human>> _wardMap;
        private MultiStateModel<Human> _model;
        private readonly IEnumerable<Ward> _wards;


        public void InitializeWithStates(MultiStateModel<Human> multiStateModel)
        {
            _model = multiStateModel;
            foreach (var region in _regions)
            {
                _counts.Add(region, new RegionCounts(multiStateModel));
            }
        }

        public void RecordTransition(Human agent, Transition<Human> transition)
        {
            var region = agent.Ward.OutputRegion;
            var regionToUpdate = _counts[region];
            regionToUpdate.TransitionCounts[transition]++;
        }

        public void RecordState(Human agent)
        {
            var region = agent.Ward.OutputRegion;
            _counts[region].StateCounts[agent.CurrentState]++;
        }

        public string CsvHeading
        {
            get
            {
                var listColumns = new List<string> {"AreaName"};

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
            var rowString = new List<string>();

            foreach (var wardRecord in _counts)
            {
                rowString.Add(wardRecord.Value.CsvString(wardRecord.Key.Name, iterationNo, day, Date));
            }

            return string.Join('\n', rowString);

        }
    }
}