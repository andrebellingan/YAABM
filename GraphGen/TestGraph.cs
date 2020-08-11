using Yaabm.Graph;

namespace GraphGen
{
    public class TestGraph : SocialContactGraph<GraphNode, GraphEdge>
    {
        protected override GraphEdge CreateEdgeInstance(GraphNode agent1, GraphNode agent2, object parameters)
        {
            return new GraphEdge(agent1, agent2);
        }
    }
}