using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using Yaabm.generic;

namespace Covid19ModelLibrary.Geography
{
    internal class TravelMatrixRow
    {
        private readonly Dictionary<int, double> _rowEntries = new Dictionary<int, double>();

        private bool _valuesChanged;
        private int[] _wards;
        private double[] _probabilities;
        private Categorical _distribution;

        public TravelMatrixRow()
        {
            _valuesChanged = true;
        }

        public void AddEntry(in int wardId, double value)
        {
            _rowEntries.Add(wardId, value);
            _valuesChanged = true;
        }

        public int SampleDestination(IRandomProvider random)
        {
            if (_valuesChanged)
            {
                _wards = new int[_rowEntries.Count];
                _probabilities = new double[_rowEntries.Count];
                var i = 0;
                foreach (var pair in _rowEntries)
                {
                    _wards[i] = pair.Key;
                    _probabilities[i] = pair.Value;
                    i++;
                }
                _distribution = new Categorical(_probabilities, random.RandomSource);
                _valuesChanged = false;
            }

            var s = _distribution.Sample();
            return _wards[s];
        }
    }
}