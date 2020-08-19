using System.Collections;
using QuickGraph.Serialization;
using Yaabm.Graph;

namespace Covid19ModelLibrary.Population
{
    public class ContactGraph : SocialContactGraph<Human, ContactEdge>
    {
        public static void SaveGraphToMl(ContactGraph graph, string fileName)
        {
            graph.SerializeToGraphML<int, ContactEdge, ContactGraph>(fileName, HumanIdentity,
                EdgeIdentities);
        }

        private static string EdgeIdentities(ContactEdge edge)
        {
            return edge.ToString();
        }

        private static string HumanIdentity(int id)
        {
            return id.ToString();
        }

        public ContactGraph(int capacity) : base(capacity)
        {
        }


        protected override ContactEdge CreateEdgeInstance(int agent1Id, int agent2Id, dynamic parameters)
        {
            return new ContactEdge(agent1Id, agent2Id, parameters.Setting);
        }
    }
}
