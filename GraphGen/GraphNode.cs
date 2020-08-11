using System;
using System.Xml.Serialization;
using Yaabm.generic;

namespace GraphGen
{
    public class GraphNode : Agent<GraphNode>
    {
        public GraphNode(int id, int group) : base(id)
        {
            GroupId = group;
        }

        [XmlAttribute]
        public int GroupId { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }

        internal static string VertexIdentities(GraphNode v)
        {
            return v.Id.ToString();
        }
    }
}
