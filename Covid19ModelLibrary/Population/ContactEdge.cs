using Yaabm.Graph;

namespace Covid19ModelLibrary.Population
{
    public class ContactEdge : AgentLink<Human>
    {
        public ContactEdge(Human source, Human target) : base(source, target)
        {
        }
    }
}