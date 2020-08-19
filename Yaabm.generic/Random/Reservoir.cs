using System.Collections.Generic;

namespace Yaabm.generic.Random
{
    public class Reservoir
    {
        public static List<int> ReservoirSampling(IEnumerable<double> weightStream, int k, IRandomProvider random)
        {
            var wSum = 0d;
            var r = new List<int>(k);

            if (k == 0) return r;

            using var enumerator = weightStream.GetEnumerator();
            for (var x = 0; x < k; x++)
            {
                enumerator.MoveNext();
                var currentWeight = enumerator.Current;
                r.Add(x);
                wSum += currentWeight;
            }

            var i = k;
            while (enumerator.MoveNext())
            {
                wSum += enumerator.Current;
                var p = enumerator.Current / wSum;
                var j = random.NextDouble();
                if (j <= p)
                {
                    var index = random.NextInt(0, k);
                    r[index] = i;
                }

                i++;
            }

            return r;
        }
    }
}