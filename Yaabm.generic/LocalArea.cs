using System;
using System.Collections.Generic;

namespace Yaabm.generic
{
    public abstract class LocalArea<T> : GeographicArea<T> where T : Agent<T>
    {
        protected LocalArea(string name, string fullName, string areaType, PopulationDynamics<T> populationDynamics) 
            : base(name, fullName, areaType)
        {
            PopulationDynamics = populationDynamics;
        }

        public PopulationDynamics<T> PopulationDynamics { get; }

        public int Day { get; set; }

        public ILocalResourceSystem<T> LocalResourceSystem { get; set; }

        public DateTime Date { get; set; }

        public override void IterateGovernanceSystem()
        {
            LocalResourceSystem.IterateOneDay(this);
        }

        public List<T> Population { get; set; } = new List<T>();
    }
}