using System;
using System.Runtime.Serialization;

namespace Yaabm.generic
{
    [DataContract(Namespace = "https://YACABM/")]
    public class WeightedChoice<T>
    {
        public WeightedChoice(T choice, double weight)
        {
            if (choice == null) throw new ArgumentNullException(nameof(choice), "Choice cannot be null");
            Choice = choice;

            Weight = weight;
        }

        [DataMember]
        public T Choice { get; set; }

        [DataMember]
        public double Weight { get; set; }
    }
}