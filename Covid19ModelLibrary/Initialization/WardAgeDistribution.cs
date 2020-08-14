using System;
using MathNet.Numerics.Distributions;
using Yaabm.generic;

namespace Covid19ModelLibrary.Initialization
{
    public class WardAgeDistribution
    {
        private readonly AgeBand[] _ageBands;
        private readonly double[] _probabilities;

        private Categorical _distribution;
        
        private bool _probabilityChanged = true;

        public WardAgeDistribution()
        {
            _ageBands = (AgeBand[]) Enum.GetValues(typeof(AgeBand));
            _probabilities = new double[_ageBands.Length];
        }

        public AgeBand[] Sample(in int agentsInThisWard, IRandomProvider random)
        {
            if (_probabilityChanged)
            {
                _distribution = new Categorical(_probabilities, random.RandomSource);
                _probabilityChanged = false;
            }

            var indexes = new int[agentsInThisWard];
            _distribution.Samples(indexes);
            var result = new AgeBand[agentsInThisWard];
            for (var i = 0; i < agentsInThisWard; i++)
            {
                result[i] = _ageBands[indexes[i]];
            }

            return result;
        }

        public double this[AgeBand ageBand]
        {
            get => GetProbability(ageBand);
            set => SetProbability(ageBand, value);
        }

        private void SetProbability(AgeBand ageBand, in double value)
        {
            var index = (int) ageBand;
            _probabilities[index] = value;
            _probabilityChanged = true;
        }

        private double GetProbability(AgeBand ageBand)
        {
            return _probabilities[(int) ageBand];
        }
    }
}