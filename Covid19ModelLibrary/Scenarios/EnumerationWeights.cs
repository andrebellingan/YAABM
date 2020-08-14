using System;
using System.Collections.Generic;
using Yaabm.generic;
using Yaabm.generic.Random;

namespace Covid19ModelLibrary.Scenarios
{
    public abstract class EnumerationWeights<T> where T: Enum
    {
        private readonly Dictionary<T, WeightedChoice<T>> _weightsDictionary = new Dictionary<T, WeightedChoice<T>>();

        private readonly List<WeightedChoice<T>> _weights = new List<WeightedChoice<T>>();

        protected EnumerationWeights()
        {
            foreach (var enumVal in (T[])Enum.GetValues(typeof(T)))
            {
                var newWeight = new WeightedChoice<T>(enumVal, 0d);
                _weightsDictionary.Add(enumVal, newWeight);
                _weights.Add(newWeight);
            }
        }

        protected double GetWeight(T enumValue)
        {
            return _weightsDictionary[enumValue].Weight;
        }

        protected void SetWeight(T enumValue, double value)
        {
            _weightsDictionary[enumValue].Weight = value;
        }

        public T Sample(IRandomProvider random)
        {
            return WeightedSampler<T>.PickSingleItem(_weights, random);
        }


    }
}