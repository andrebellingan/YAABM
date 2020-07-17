namespace Yaabm.generic
{
    public abstract class CustomContext<T> where T : Agent<T>
    {
        protected CustomContext(string name, GroupedContext<T> parent)
        {
            Parent = parent;
            Name = name;
        }

        public GroupedContext<T> Parent { get; set; }

        public string Name { get; }

        public abstract void IterateGovernanceSystem();
    }
}