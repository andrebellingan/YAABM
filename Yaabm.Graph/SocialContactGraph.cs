using QuickGraph;
using Yaabm.generic;

namespace Yaabm.Graph
{
    public abstract class SocialContactGraph<TAgent, TEdge> : UndirectedGraph<TAgent, TEdge> where TAgent : Agent<TAgent> where TEdge : AgentLink<TAgent>
    {
        public TEdge ConnectAgents(TAgent agent1, TAgent agent2, dynamic parameters)
        {
            var newLink = CreateEdgeInstance(agent1, agent2, parameters);
            AddEdge(newLink);
            return newLink;
        }

        public TEdge ConnectAgents(TAgent agent1, TAgent agent2)
        {
            return ConnectAgents(agent1, agent2, null);
        }


        protected abstract TEdge CreateEdgeInstance(TAgent agent1, TAgent agent2, object parameters);

        protected SocialContactGraph() : base(false, BiDirectionalEdgeComparer)
        {
        }

        private static bool BiDirectionalEdgeComparer(TEdge edge, TAgent source, TAgent target)
        {
            var oneWay = edge.Source == source && edge.Target == target;
            var otherWay = edge.Source == target && edge.Target == source;
            return oneWay || otherWay;
        }

    }
}
