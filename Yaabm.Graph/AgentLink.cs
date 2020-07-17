using QuickGraph;
using Yaabm.generic;

namespace Yaabm.Graph
{
    public class AgentLink<TAgent> : UndirectedEdge<TAgent>
        where TAgent : Agent<TAgent> 
    {
        public AgentLink(TAgent source, TAgent target) : base(source, target)
        {
        }

        public TAgent OtherConnectedAgent(TAgent agent)
        {
            if (Source == agent) return Target;
            if (Target == agent) return Source;
            return null;
        }
    }
}