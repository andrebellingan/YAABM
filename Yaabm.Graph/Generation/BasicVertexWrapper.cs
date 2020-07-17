using Yaabm.generic;

namespace Yaabm.Graph.Generation
{
    public class BasicVertexWrapper<TAgent> where TAgent : Agent<TAgent>
    {
        public BasicVertexWrapper(TAgent agent, int desiredDegree)
        {
            Agent = agent;
            DesiredDegree = desiredDegree;

            if (DesiredDegree > 0) IsFree = true;
        }

        public int AgentId => Agent.Id;

        public TAgent Agent { get; }

        public int DesiredDegree { get; }

        private int _actualDegree;

        public int ActualDegree
        {
            get => _actualDegree;
            set
            {
                _actualDegree = value;
                if (_actualDegree == DesiredDegree) IsFree = false;
            }
        }

        public bool IsFree { get; private set; }

    }
}
