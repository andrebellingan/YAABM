using System.Xml.Serialization;
using Yaabm.Graph;

namespace Covid19ModelLibrary.Population
{
    public class ContactEdge : AgentLink
    {
        public ContactEdge(int sourceId, int targetId, ContactSetting setting) : base(sourceId, targetId)
        {
            ContactSetting = setting;
        }

        public ContactSetting ContactSetting { get; set; }

        [XmlAttribute] 
        // ReSharper disable once UnusedMember.Global
        public string Setting => ContactSetting.ToString();
    }
}