using System.Collections.Generic;
using Yaabm.generic;

namespace Yaabm.Graph.Generation
{
    public class MultiVertexWrapper<TAgent> where TAgent: Agent<TAgent>
    {
        private readonly List<int> _actualDegrees = new List<int>();
        private readonly List<bool> _isFree = new List<bool>();

        public MultiVertexWrapper(TAgent agent, List<int> desiredDegrees, int groupId)
        {
            Agent = agent;
            DesiredDegrees = desiredDegrees;
            GroupId = groupId;

            foreach (var t in desiredDegrees)
            {
                _actualDegrees.Add(0);
                _isFree.Add(t > 0);
            }
        }

        public int GroupId { get; set; }

        public TAgent Agent { get; }

        public List<int> DesiredDegrees { get; }

        public int AgentId => Agent.Id;

        public bool IsFree(int groupId)
        {
            return _isFree[groupId];
        }

        public bool IsFree()
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var value in _isFree)
            {
                if (value) return true;
            }

            return false;
        }

        public void IncrementActualDegree(int groupId)
        {
            _actualDegrees[groupId]++;
            if (_actualDegrees[groupId] >= DesiredDegrees[groupId]) _isFree[groupId] = false;
        }
    }
}
