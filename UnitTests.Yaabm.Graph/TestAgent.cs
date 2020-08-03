using System;
using Yaabm.generic;

namespace UnitTests.Yaabm.Graph
{
    public class TestAgent : Agent<TestAgent>
    {
        public TestAgent(int uid) : base (uid)
        {
            Uid = uid;
        }

        public int Uid { get; set; }

        public override void Behave()
        {
            throw new NotImplementedException();
        }
    }
}
