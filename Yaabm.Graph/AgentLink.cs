using QuickGraph; 
using Yaabm.generic;

namespace Yaabm.Graph
{
    public class AgentLink : UndirectedEdge<int>
    {
        public AgentLink(int source, int target) : base(source, target)
        {
        }

        public int OtherConnectedAgent(int agent)
        {
            return Source == agent ? Target : Source;
        }
    }
}