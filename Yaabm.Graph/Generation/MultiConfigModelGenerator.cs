using System;
using System.Collections.Generic;
using System.Linq;
using Loyc.Collections;
using Yaabm.generic;

namespace Yaabm.Graph.Generation
{
    public class MultiConfigItem<TAgent>
    {
        public TAgent Agent { get; set; }
        
        public List<int> Degrees { get; set; }

        public int GroupId { get; set; }
    }

    public class MultiConfigModelGenerator<TAgent, TEdge, TGraph>
        where TAgent : Agent<TAgent>
        where TEdge : AgentLink<TAgent>
        where TGraph : SocialContactGraph<TAgent, TEdge>
    {
        private class WrapperDictionary : Dictionary<int, MultiVertexWrapper<TAgent>>
        {
        }

        private readonly WrapperDictionary _vertexWrappers = new WrapperDictionary();

        private readonly WrapperDictionary _needAnyConnections = new WrapperDictionary();

        private readonly Dictionary<int, AList<MultiVertexWrapper<TAgent>>> _freeVerticesByGroup = new Dictionary<int, AList<MultiVertexWrapper<TAgent>>>();

        private readonly IRandomProvider _random;


        public MultiConfigModelGenerator(int numberOfGroups, TGraph graph, IRandomProvider random)
        {
            NumberOfGroups = numberOfGroups;
            Graph = graph;
            _random = random;
        }

        public TGraph Graph { get; set; }

        public void GenerateLinks(IList<MultiConfigItem<TAgent>> agentsAndDegrees, bool resetGraph)
        {
            if (resetGraph)
            {
                _vertexWrappers.Clear();
                ClearFreeVertexWrappers();
            }

            foreach (var item in agentsAndDegrees)
            {
                if (Graph.ContainsVertex(item.Agent))
                {
                    throw new InvalidOperationException("This agent has already been added to the graph");
                }
                Graph.AddVertex(item.Agent);
                var newWrapper = new MultiVertexWrapper<TAgent>(item.Agent, item.Degrees, item.GroupId);
                _vertexWrappers.Add(newWrapper.AgentId, newWrapper);

                var needAnyConnections = false;
                for (var g = 0; g < NumberOfGroups; g++)
                {
                    if (newWrapper.IsFree(g))
                    {
                        _freeVerticesByGroup[g].Add(newWrapper);
                        needAnyConnections = true;
                    }
                }

                if (needAnyConnections) _needAnyConnections.Add(newWrapper.AgentId, newWrapper);
            }

            var wrapperKeys = _needAnyConnections.Keys.ToArray();


            int[] randomOrder;
            if (Shuffle)
            {
                randomOrder = _random.Shuffle(wrapperKeys.Length);
            }
            else
            {
                randomOrder = new int[wrapperKeys.Length];
                for (var z = 0; z < wrapperKeys.Length; z++)
                {
                    randomOrder[z] = z;
                }
            }

            for (var i = 0; i < wrapperKeys.Length; i++)
            {
                var randomIndex = randomOrder[i];
                var wrapperKey = wrapperKeys[randomIndex];
                var wrapper = _vertexWrappers[wrapperKey];
                GenerateEdges(wrapper);
            }

            TrimNeedAnyConnectionsList();
        }

        private void TrimNeedAnyConnectionsList()
        {
            var indexesToDelete = new List<int>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var pair in _needAnyConnections)
            {
                if (!pair.Value.IsFree())
                {
                    indexesToDelete.Add(pair.Key);
                }
            }

            _needAnyConnections.RemoveRange(indexesToDelete);
        }

        public int MaximumRetries { get; set; } = 100;

        public bool Shuffle { get; set; } = true;

        private void GenerateEdges(MultiVertexWrapper<TAgent> wrapper)
        {
            var currentAgent = wrapper.Agent;
            var freeToConnectToAgent = _freeVerticesByGroup[wrapper.GroupId];

            for (var groupIndex = 0; groupIndex < NumberOfGroups; groupIndex++)
            {
                var isEnough = !wrapper.IsFree(groupIndex);

                if (isEnough) continue;

                var retryNumber = 0;

                while (!isEnough)
                {
                    retryNumber++;
                    if (retryNumber > MaximumRetries || freeToConnectToAgent.Count <= 1)
                    {
                        isEnough = true;
                        continue;
                    }

                    var randomIndex = _random.NextInt(0, freeToConnectToAgent.Count);
                    var otherWrapper = freeToConnectToAgent[randomIndex];

                    if(!LinkingConditionMet(wrapper, otherWrapper, groupIndex)) continue;

                    Graph.ConnectAgents(currentAgent, otherWrapper.Agent);
                    wrapper.IncrementActualDegree(groupIndex);
                    otherWrapper.IncrementActualDegree(wrapper.GroupId);

                    if (!otherWrapper.IsFree(wrapper.GroupId))
                    {
                        freeToConnectToAgent.RemoveAt(randomIndex);
                    }

                    isEnough = !wrapper.IsFree(groupIndex);
                }
            }
        }

        private bool LinkingConditionMet(MultiVertexWrapper<TAgent> wrapper, MultiVertexWrapper<TAgent> otherWrapper,
            int groupIndex)
        {
            if (otherWrapper.AgentId == wrapper.AgentId) return false; // can't link to yourself
            if (otherWrapper.GroupId != groupIndex) return false; // this guy is not from the right group
            if (Graph.ContainsEdge(wrapper.Agent, otherWrapper.Agent)) return false; // that link already exists
            if (!otherWrapper.IsFree(wrapper.GroupId)) return false; // the other guy already has enough connections of this type
            return true;
        }

        private void ClearFreeVertexWrappers()
        {
            _freeVerticesByGroup.Clear();
            for (var g = 0; g < NumberOfGroups; g++)
            {
                _freeVerticesByGroup.Add(g, new AList<MultiVertexWrapper<TAgent>>());
            }
        }

        public int NumberOfGroups { get; }
    }
}
