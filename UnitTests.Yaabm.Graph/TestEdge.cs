using Yaabm.Graph;

namespace UnitTests.Yaabm.Graph
{
    public class TestEdge : AgentLink<TestAgent>
    {
        public TestEdge(TestAgent source, TestAgent target) : base(source, target)
        {
            Description = $"{source.Id}--{target.Id}";
        }

        public string Description { get; }
    }
}
