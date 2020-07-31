using System;

namespace Yaabm.generic
{
    public abstract class LocalArea<T> : CustomContext<T> where T : Agent<T>
    {
        protected LocalArea(ILocalResourceSystem<T> localResource, string name, GroupedContext<T> parentContext) 
            : base(name, parentContext)
        {
            LocalResourceSystem = localResource;
        }

        public int Day { get; set; }

        public ILocalResourceSystem<T> LocalResourceSystem { get; }

        public DateTime Date { get; set; }

        public override void IterateGovernanceSystem()
        {
            LocalResourceSystem.IterateOneDay(this);
        }
    }
}