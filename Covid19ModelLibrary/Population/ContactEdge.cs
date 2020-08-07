using System.Xml.Serialization;
using Yaabm.Graph;

namespace Covid19ModelLibrary.Population
{
    public class ContactEdge : AgentLink<Human>
    {
        public ContactEdge(Human source, Human target, ContactSetting setting) : base(source, target)
        {
            ContactSetting = setting;
        }

        public ContactSetting ContactSetting { get; set; }

        [XmlAttribute] 
        public string Setting => ContactSetting.ToString();
    }
}