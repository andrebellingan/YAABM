using System;
using System.Collections.Generic;
using System.Linq;
using Loyc.Collections;
using Yaabm.generic;

namespace Yaabm.Graph.Generation
{
    public class SimpleConfigModelGenerator<TAgent, TEdge, TGraph> 
        where TAgent: Agent<TAgent> 
        where TEdge : AgentLink<TAgent>
        where TGraph: SocialContactGraph<TAgent, TEdge>
    {
        private readonly Dictionary<int, BasicVertexWrapper<TAgent>> _vertexWrappers = new Dictionary<int, BasicVertexWrapper<TAgent>>();
        private readonly AList<BasicVertexWrapper<TAgent>> _freeVertices = new AList<BasicVertexWrapper<TAgent>>();
        private readonly Dictionary<int, BasicVertexWrapper<TAgent>> _needAnyConnections = new Dictionary<int, BasicVertexWrapper<TAgent>>();
        private readonly IRandomProvider _random;

        public SimpleConfigModelGenerator(TGraph graph, IRandomProvider random)
        {
            Graph = graph;
            _random = random;
        }

        public TGraph Graph { get; set; }

        public int MaximumRetries { get; set; } = 100;

        public void GenerateLinks(IList<Tuple<TAgent, int>> agentsAndDegree, bool resetGraph)
        {
            if (resetGraph)
            {
                _vertexWrappers.Clear();
                _freeVertices.Clear();
            }

            foreach (var (agent, degree) in agentsAndDegree)
            {
                if (Graph.ContainsVertex(agent))
                {
                    throw new InvalidOperationException("This agent ha already been added to the graph");
                }
                Graph.AddVertex(agent);
                var newWrapper = new BasicVertexWrapper<TAgent>(agent, degree);
                _vertexWrappers.Add(newWrapper.AgentId, newWrapper);
                if (newWrapper.IsFree) 
                {
                    _freeVertices.Add(newWrapper);
                    _needAnyConnections.Add(newWrapper.AgentId, newWrapper);
                }
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

            foreach (var pair in _needAnyConnections)
            {
                if (!pair.Value.IsFree)
                {
                    indexesToDelete.Add(pair.Key);
                }
            }

            _needAnyConnections.RemoveRange(indexesToDelete);
        }

        public bool Shuffle { get; set; } = true;

        private void GenerateEdges(BasicVertexWrapper<TAgent> wrapper)
        {
            var currentAgent = wrapper.Agent;
            var freeToConnect = _freeVertices;

            var isEnough = !wrapper.IsFree;

            if (isEnough) return;

            var retryNumber = 0;

            while (!isEnough)
            {
                retryNumber++;
                if (retryNumber > MaximumRetries || freeToConnect.Count <= 1)
                {
                    isEnough = true;
                    continue;
                }

                var randomIndex = _random.NextInt(0, freeToConnect.Count);
                var otherWrapper = freeToConnect[randomIndex];

                if (otherWrapper.AgentId == wrapper.AgentId) continue; // can't link to yourself
                if (Graph.ContainsEdge(currentAgent, otherWrapper.Agent)) continue; // that link already exists
                if (!otherWrapper.IsFree) continue; // the other guy already has enough connections of this type

                Graph.ConnectAgents(currentAgent, otherWrapper.Agent);
                wrapper.ActualDegree++;
                otherWrapper.ActualDegree++;

                if (!otherWrapper.IsFree)
                {
                    freeToConnect.RemoveAt(randomIndex);
                }

                isEnough = !wrapper.IsFree;
            }
        }
    }
}
