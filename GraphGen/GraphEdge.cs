using Yaabm.Graph;

namespace GraphGen
{
    public class GraphEdge : AgentLink<GraphNode>
    {
        public GraphEdge(GraphNode source, GraphNode target) : base(source, target)
        {
        }
    }
}