using QuickGraph.Serialization;
using Yaabm.Graph;

namespace Covid19ModelLibrary.Population
{
    public class ContactGraph : SocialContactGraph<Human, ContactEdge>
    {
        public ContactGraph(ContactSetting setting) 
        {
            Setting = setting;
        }

        public ContactSetting Setting { get; set; }

        protected override ContactEdge CreateEdgeInstance(Human agent1, Human agent2)
        {
            return new ContactEdge(agent1, agent2);
        }

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
    }
}
