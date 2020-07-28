using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using QuickGraph;
using QuickGraph.Serialization;
using Yaabm.generic;

namespace Yaabm.Graph
{
    public abstract class SocialContactGraph<TAgent, TEdge> : UndirectedGraph<TAgent, TEdge> where TAgent : Agent<TAgent> where TEdge : AgentLink<TAgent>
    {
        public void ConnectAgents(TAgent agent1, TAgent agent2)
        {
            var newLink = CreateEdgeInstance(agent1, agent2);
            AddEdge(newLink);
        }

        protected abstract TEdge CreateEdgeInstance(TAgent agent1, TAgent agent2);

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
