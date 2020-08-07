using QuickGraph.Serialization;
using Yaabm.Graph;

namespace Covid19ModelLibrary.Population
{
    public class ContactGraph : SocialContactGraph<Human, ContactEdge>
    {
        public static void SaveGraphToMl(ContactGraph graph, string fileName)
        {
            graph.SerializeToGraphML<Human, ContactEdge, ContactGraph>(fileName, HumanIdentity,
                EdgeIdentities);
        }

        private static string EdgeIdentities(ContactEdge edge)
        {
            return edge.ToString();
        }

        private static string HumanIdentity(Human human)
        {
            return human.Id.ToString();
        }

        protected override ContactEdge CreateEdgeInstance(Human agent1, Human agent2, dynamic parameters)
        {
            return new ContactEdge(agent1, agent2, parameters.Setting);
        }
    }
}
