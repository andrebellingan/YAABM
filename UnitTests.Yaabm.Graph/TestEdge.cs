using Yaabm.Graph;

namespace UnitTests.Yaabm.Graph
{
    public class TestEdge : AgentLink
    {
        public TestEdge(int sourceId, int targetId) : base(sourceId, targetId)
        {
            Description = $"{sourceId}--{targetId}";
        }

        public string Description { get; }
    }
}
