using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using QuickGraph;
using QuickGraph.Collections;
using Yaabm.generic;

namespace Yaabm.Graph
{
    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class XUndirectedGraph<TVertex, TEdge> : IMutableUndirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private readonly VertexEdgeDictionary<TVertex, TEdge> _adjacentEdges;
        private EdgeEqualityComparer<TVertex, TEdge> _edgeEqualityComparer;

        public XUndirectedGraph(int capacity, bool allowParallelEdges, EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer)
        {
            AllowParallelEdges = allowParallelEdges;
            _edgeEqualityComparer = edgeEqualityComparer;
            _adjacentEdges = new VertexEdgeDictionary<TVertex, TEdge>(capacity);
        }

        public XUndirectedGraph(bool allowParallelEdges, EdgeEqualityComparer<TVertex, TEdge> edgeEqualityComparer)
        {
            AllowParallelEdges = allowParallelEdges;
            _edgeEqualityComparer = edgeEqualityComparer;
            _adjacentEdges = new VertexEdgeDictionary<TVertex, TEdge>();
        }

        public XUndirectedGraph(bool allowParallelEdges) : this(allowParallelEdges, EdgeExtensions.GetUndirectedVertexEquality<TVertex, TEdge>())
        {
            AllowParallelEdges = allowParallelEdges;
        }

        public XUndirectedGraph() : this(true)
        { }

        public EdgeEqualityComparer<TVertex, TEdge> EdgeEqualityComparer
        {
            get
            {
                Contract.Ensures(Contract.Result<EdgeEqualityComparer<TVertex, TEdge>>() != null);
                return _edgeEqualityComparer;
            }
        }

        public int EdgeCapacity { get; set; } = 4;

        public IEnumerable<TEdge> AdjacentEdges(TVertex v)
        {
            return _adjacentEdges[v];
        }

        public IEnumerable<TVertex> AdjacentVertices(TVertex v)
        {
            var adjacentEdges = AdjacentEdges(v);
            var adjacentVertices = new HashSet<TVertex>();
            foreach (TEdge edge in adjacentEdges)
            {
                adjacentVertices.Add(edge.Source);
                adjacentVertices.Add(edge.Target);
            }

            adjacentVertices.Remove(v);

            return adjacentVertices;
        }

        public bool RemoveVertex(TVertex v)
        {
            ClearAdjacentEdges(v);
            bool result = _adjacentEdges.Remove(v);

            if (result)
                OnVertexRemoved(v);

            return result;
        }

        public int RemoveVertexIf(VertexPredicate<TVertex> pred)
        {
            var vertices = new List<TVertex>();
            foreach (var v in this.Vertices)
                if (pred(v))
                    vertices.Add(v);

            foreach (var v in vertices)
                RemoveVertex(v);
            return vertices.Count;
        }

        public void ClearAdjacentEdges(TVertex v)
        {
            var edges = this._adjacentEdges[v].Clone();
            EdgeCount -= edges.Count;

            foreach (var edge in edges)
            {
                IEdgeList<TVertex, TEdge> aEdges;
                if (_adjacentEdges.TryGetValue(edge.Target, out aEdges))
                    aEdges.Remove(edge);
                if (_adjacentEdges.TryGetValue(edge.Source, out aEdges))
                    aEdges.Remove(edge);
            }
        }

        public void Clear()
        {
            _adjacentEdges.Clear();
            EdgeCount = 0;
        }

        public bool AddEdge(TEdge edge)
        {
            var sourceEdges = _adjacentEdges[edge.Source];
            if (!AllowParallelEdges)
            {
                if (ContainsEdgeBetweenVertices(sourceEdges, edge))
                    return false;
            }

            sourceEdges.Add(edge);
            if (!EdgeExtensions.IsSelfEdge<TVertex, TEdge>(edge))
            {
                var targetEdges = _adjacentEdges[edge.Target];
                targetEdges.Add(edge);
            }
            EdgeCount++;
            OnEdgeAdded(edge);

            return true;
        }

        public bool AllowParallelEdges { get; }

        public event EdgeAction<TVertex, TEdge> EdgeAdded;
        protected virtual void OnEdgeAdded(TEdge args)
        {
            var eh = EdgeAdded;
            eh?.Invoke(args);
        }

        private bool ContainsEdgeBetweenVertices(IEnumerable<TEdge> edges, TEdge edge)
        {
            var source = edge.Source;
            var target = edge.Target;
            foreach (var e in edges)
                if (this.EdgeEqualityComparer(e, source, target))
                    return true;
            return false;
        }

        public bool ContainsVertex(TVertex vertex)
        {
            return _adjacentEdges.ContainsKey(vertex);
        }

        public IEnumerable<TVertex> Vertices => _adjacentEdges.Keys;

        public bool ContainsEdge(TEdge edge)
        {
            var eqc = this.EdgeEqualityComparer;
            foreach (var e in this.AdjacentEdges(edge.Source))
                if (e.Equals(edge))
                    return true;
            return false;
        }

        public bool ContainsEdge(TVertex source, TVertex target)
        {
            TEdge edge;
            return TryGetEdge(source, target, out edge);
        }

        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (Comparer<TVertex>.Default.Compare(source, target) > 0)
            {
                var temp = source;
                source = target;
                target = temp;
            }

            foreach (var e in this.AdjacentEdges(source))
            {
                if (_edgeEqualityComparer(e, source, target))
                {
                    edge = e;
                    return true;
                }
            }

            edge = default(TEdge);
            return false;
        }

        public event VertexAction<TVertex> VertexRemoved;
        protected virtual void OnVertexRemoved(TVertex args)
        {
            Contract.Requires(args != null);

            var eh = this.VertexRemoved;
            if (eh != null)
                eh(args);
        }

        public bool AddVertex(TVertex v)
        {
            if (ContainsVertex(v))
                return false;

            var edges = this.EdgeCapacity < 0
                ? new EdgeList<TVertex, TEdge>()
                : new EdgeList<TVertex, TEdge>(this.EdgeCapacity);
            _adjacentEdges.Add(v, edges);
            OnVertexAdded(v);
            return true;
        }

        public event VertexAction<TVertex> VertexAdded;
        protected virtual void OnVertexAdded(TVertex args)
        {
            Contract.Requires(args != null);

            var eh = this.VertexAdded;
            eh?.Invoke(args);
        }

        public bool IsDirected => false;

        public int VertexCount => _adjacentEdges.Count;

        public bool IsVerticesEmpty => _adjacentEdges.Count == 0;

        public IEnumerable<TEdge> Edges
        {
            get
            {
                var edgeColors = new Dictionary<TEdge, GraphColor>(EdgeCount);
                foreach (var edges in _adjacentEdges.Values)
                {
                    foreach (TEdge edge in edges)
                    {
                        GraphColor c;
                        if (edgeColors.TryGetValue(edge, out c))
                            continue;
                        edgeColors.Add(edge, GraphColor.Black);
                        yield return edge;
                    }
                }
            }
        }

        public bool IsEdgesEmpty => EdgeCount == 0;

        public int EdgeCount { get; private set; } = 0;

        public event EdgeAction<TVertex, TEdge> EdgeRemoved;
        protected virtual void OnEdgeRemoved(TEdge args)
        {
            var eh = EdgeRemoved;
            eh?.Invoke(args);
        }

        public int RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
        {
            List<TEdge> edges = new List<TEdge>();
            foreach (var edge in this.Edges)
            {
                if (predicate(edge))
                    edges.Add(edge);
            }
            return this.RemoveEdges(edges);
        }

        public int RemoveEdges(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
            {
                if (RemoveEdge(edge))
                    count++;
            }
            return count;
        }

        public bool RemoveEdge(TEdge edge)
        {
            bool removed = this._adjacentEdges[edge.Source].Remove(edge);
            if (removed)
            {
                if (!EdgeExtensions.IsSelfEdge<TVertex, TEdge>(edge))
                    this._adjacentEdges[edge.Target].Remove(edge);
                this.EdgeCount--;
                Contract.Assert(this.EdgeCount >= 0);
                this.OnEdgeRemoved(edge);
                return true;
            }
            else
                return false;
        }

        public int AddEdgeRange(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
                if (this.AddEdge(edge))
                    count++;
            return count;
        }

        public int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            int count = 0;
            foreach (var v in vertices)
                if (this.AddVertex(v))
                    count++;
            return count;
        }

        public int AddVerticesAndEdgeRange(IEnumerable<TEdge> edges)
        {
            int count = 0;
            foreach (var edge in edges)
                if (this.AddVerticesAndEdge(edge))
                    count++;
            return count;
        }

        public bool AddVerticesAndEdge(TEdge edge)
        {
            var sourceEdges = this.AddAndReturnEdges(edge.Source);
            var targetEdges = this.AddAndReturnEdges(edge.Target);

            if (!this.AllowParallelEdges)
            {
                if (this.ContainsEdgeBetweenVertices(sourceEdges, edge))
                    return false;
            }

            sourceEdges.Add(edge);
            if (!EdgeExtensions.IsSelfEdge<TVertex, TEdge>(edge))
                targetEdges.Add(edge);
            this.EdgeCount++;
            this.OnEdgeAdded(edge);

            return true;
        }

        private IEdgeList<TVertex, TEdge> AddAndReturnEdges(TVertex v)
        {
            IEdgeList<TVertex, TEdge> edges;
            if (!this._adjacentEdges.TryGetValue(v, out edges))
                this._adjacentEdges[v] = edges = this.EdgeCapacity < 0
                    ? new EdgeList<TVertex, TEdge>()
                    : new EdgeList<TVertex, TEdge>(this.EdgeCapacity);

            return edges;
        }

        public TEdge AdjacentEdge(TVertex v, int index)
        {
            return this._adjacentEdges[v][index];
        }

        public bool IsAdjacentEdgesEmpty(TVertex v)
        {
            return this._adjacentEdges[v].Count == 0;
        }

        public int AdjacentDegree(TVertex v)
        {
            return this._adjacentEdges[v].Count;
        }

        public int RemoveAdjacentEdgeIf(TVertex v, EdgePredicate<TVertex, TEdge> predicate)
        {
            var outEdges = this._adjacentEdges[v];
            var edges = new List<TEdge>(outEdges.Count);
            foreach (var edge in outEdges)
                if (predicate(edge))
                    edges.Add(edge);

            this.RemoveEdges(edges);
            return edges.Count;
        }
    }
}
