using Yaabm.Graph;

namespace UnitTests.Yaabm.Graph
{
    public class TestGraph : SocialContactGraph<TestAgent, TestEdge>
    {
        protected TestGraph(int capacity) : base(capacity)
        {
        }

        protected override TestEdge CreateEdgeInstance(int agent1Id, int agent2Id, dynamic parameters)
        {
           return new TestEdge(agent1Id, agent2Id);
        }
    }
}
