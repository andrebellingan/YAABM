using System;
using Yaabm.generic;

namespace GraphGen
{
    public class GraphNode : Agent<GraphNode>
    {
        public GraphNode(int id, int group) : base(id)
        {
            GroupId = group;
        }

        public int GroupId { get; set; }

        public override void Behave()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
