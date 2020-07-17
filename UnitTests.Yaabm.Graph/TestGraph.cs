using Yaabm.Graph;

namespace UnitTests.Yaabm.Graph
{
    public class TestGraph : SocialContactGraph<TestAgent, TestEdge>
    {
        protected override TestEdge CreateEdgeInstance(TestAgent agent1, TestAgent agent2)
        {
            return new TestEdge(agent1, agent2);
        }
    }
}
