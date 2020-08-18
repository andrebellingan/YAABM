using Yaabm.generic;

namespace Yaabm.Graph
{
    public abstract class SocialContactGraph<TAgent, TEdge> : XUndirectedGraph<int, TEdge> where TAgent : Agent<TAgent> where TEdge : AgentLink
    {
        public TEdge ConnectAgents(int agent1Id, int agent2Id, dynamic parameters)
        {
            var newLink = CreateEdgeInstance(agent1Id, agent2Id, parameters);
            AddEdge(newLink);
            return newLink;
        }

        public TEdge ConnectAgents(TAgent agent1, TAgent agent2)
        {
            return ConnectAgents(agent1.Id, agent2.Id, null);
        }

        public TEdge ConnectAgents(int agent1Id, int agent2Id)
        {
            return ConnectAgents(agent1Id, agent1Id, null);
        }

        public bool AddVertex(TAgent newVertex)
        {
            return AddVertex(newVertex.Id);
        }

        protected abstract TEdge CreateEdgeInstance(int agent1Id, int agent2Id, object parameters);

        protected SocialContactGraph(int capacity) : base(capacity, false, BiDirectionalEdgeComparer)
        {
        }

        private static bool BiDirectionalEdgeComparer(TEdge edge, int source, int target)
        {
            var oneWay = edge.Source == source && edge.Target == target;
            var otherWay = edge.Source == target && edge.Target == source;
            return oneWay || otherWay;
        }

    }
}
