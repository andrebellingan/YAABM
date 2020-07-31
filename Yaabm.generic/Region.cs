using System.Collections.Generic;

namespace Yaabm.generic
{
    public class Region<T> : GeographicArea<T> where T : Agent<T>
    {
        internal Region(string name, string fullName, string areaType)
            : base(name, fullName, areaType)
        {
        }

        internal Dictionary<string, GeographicArea<T>> ChildContexts { get; } = new Dictionary<string, GeographicArea<T>>();

        public override void IterateGovernanceSystem()
        {
            foreach (var child in ChildContexts) child.Value.IterateGovernanceSystem();
        }

        internal void AddChild(GeographicArea<T> childContext)
        {
            ChildContexts.Add(childContext.Name, childContext);
            childContext.Parent = this;
        }
    }
}