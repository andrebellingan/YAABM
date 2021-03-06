﻿using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace Covid19ModelLibrary.Initialization
{
    public class WardHouseSizeDistribution
    {
        private readonly double[] _householdCounts;

        private readonly Dictionary<int, Categorical> _conditionalDistributions = new Dictionary<int, Categorical>();

        private bool _valuesChanged = true;

        public WardHouseSizeDistribution(int maxHouseHoldSize)
        {
            _householdCounts = new double[maxHouseHoldSize+1];
            MaximumSize = maxHouseHoldSize;
        }

        public double this[int size]
        {
            get => _householdCounts[size];
            set
            {
                _householdCounts[size] = value;
                _valuesChanged = true;
            }
        }

        public int MaximumSize { get; }

        private readonly object _sampleLock = new object();

        internal int Sample(int maximumSize, Random random)
        {
            if (maximumSize == 0) throw new ArgumentOutOfRangeException(nameof(maximumSize), "Cannot sample households with size=0");
            if (maximumSize > MaximumSize) throw new ArgumentOutOfRangeException(nameof(maximumSize), $"Cannot sample household sizes larger than {MaximumSize}");

            CheckIfValuesChanged(random);

            var distribution = _conditionalDistributions[maximumSize];
            return distribution.Sample();
        }

        private void CheckIfValuesChanged(Random random)
        {
            lock (_sampleLock)
            {
                if (!_valuesChanged) return;

                CalculateConditionalDistributions(random);
                _valuesChanged = false;
            }
        }

        private void CalculateConditionalDistributions(Random random)
        {
            _conditionalDistributions.Clear();

            for (var i = 1; i <= MaximumSize; i++)
            {
                var weights = GetWeights(i);
                var scaledWeights = GetScaledWeights(weights);
                var distribution = new Categorical(scaledWeights, random);
                _conditionalDistributions.Add(i, distribution);
            }
        }

        private static double[] GetScaledWeights(IReadOnlyList<double> weights)
        {
            var scaled = new double[weights.Count];
            var grandTotal = weights.Sum();
            for (var i = 0; i < weights.Count; i++)
            {
                scaled[i] = weights[i] / grandTotal;
            }

            return scaled;
        }

        private double[] GetWeights(in int maxSize)
        {
            var result = new double[maxSize+1];

            for (var s = 0; s <= maxSize; s++)
            {
                result[s] = _householdCounts[s];
            }

            return result;
        }
    }
}