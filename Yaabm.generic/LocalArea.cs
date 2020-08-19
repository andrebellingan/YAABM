using System;
using System.Collections.Generic;

namespace Yaabm.generic
{
    public abstract class LocalArea<T> : GeographicArea<T> where T : Agent<T>
    {
        protected LocalArea(string name, string fullName, string areaType, PopulationDynamics<T> environment) 
            : base(name, fullName, areaType)
        {
            Environment = environment;
        }

        public PopulationDynamics<T> Environment { get; }

        public int Day { get; set; }

        public ILocalResourceSystem<T> LocalResourceSystem { get; set; }

        public DateTime Date { get; set; }

        public List<int> ResidentIds { get; set; } = new List<int>();
    }
}