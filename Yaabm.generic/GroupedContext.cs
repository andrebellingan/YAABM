using System.Collections.Generic;

namespace Yaabm.generic
{
    public class GroupedContext<T> : CustomContext<T> where T : Agent<T>
    {
        internal GroupedContext(string name, GroupedContext<T> parentContext) : base(name,
            parentContext)
        {
            ParentContext = parentContext;
        }

        public GroupedContext<T> ParentContext { get; }

        internal Dictionary<string, CustomContext<T>> ChildContexts { get; } = new Dictionary<string, CustomContext<T>>();

        public override void IterateGovernanceSystem()
        {
            foreach (var child in ChildContexts) child.Value.IterateGovernanceSystem();
        }

        internal void AddChild(CustomContext<T> childContext)
        {
            ChildContexts.Add(childContext.Name, childContext);
        }
    }
}