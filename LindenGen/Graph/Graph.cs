using System.Collections;
using System.Collections.Generic;

namespace LindenGen.Graph
{
    public class Graph<T> : IEnumerable<Vertex<T>>
    {
        private static int vertexID;

        public IDictionary<int, Vertex<T>> Vertices { get; }
        public IDictionary<int, HashSet<int>> Edges { get; }

        public Graph() {
            Vertices = new Dictionary<int, Vertex<T>>();
            Edges = new Dictionary<int, HashSet<int>>();
        }

        internal Graph(T first, IEnumerable<ContinuingTerm<T>> following)
        {
            Vertices = new Dictionary<int, Vertex<T>>();
            Edges = new Dictionary<int, HashSet<int>>();

            Vertex<T> current = AddVertex(first);
            foreach (ContinuingTerm<T> term in following)
            {
                Vertex<T> next = AddVertex(term.Term);
                AddEdge(current, next);
                current = next;
            }
        }

        public Vertex<T> AddVertex(T value)
        {
            Vertex<T> found;
            if ((found = FindByValue(value)) != null)
                return found;

            Vertex<T> newVertex = new Vertex<T>(vertexID++, value);
            Vertices.Add(newVertex.ID, newVertex);
            Edges.Add(newVertex.ID, new HashSet<int>());

            return newVertex;
        }

        public void AddVertex(Vertex<T> vertex)
        {
            if (!Vertices.ContainsKey(vertex.ID)) {
                Vertices.Add(vertex.ID, vertex);
                Edges.Add(vertex.ID, new HashSet<int>());
            }
        }

        public bool RemoveVertex(int id)
        {
            if (!Vertices.TryGetValue(id, out Vertex<T> vertex))
                return false;

            // remove outgoing edges
            foreach (int neighborID in Edges[id])
                RemoveEdge(id, neighborID);

            // remove incoming edges
            foreach (int srcID in Vertices.Keys)
                RemoveEdge(srcID, id);

            Edges.Remove(id);
            Vertices.Remove(id);
            return true;
        }

        public void AddEdge(int srcID, int destID)
        {
            AddEdge(Vertices[srcID], Vertices[destID]);
        }

        public void AddEdge(Vertex<T> src, Vertex<T> dest)
        {
            AddVertex(src);
            AddVertex(dest);
            Edges[src.ID].Add(dest.ID);
        }

        public bool RemoveEdge(int srcID, int destID)
        {
            if (!Vertices.TryGetValue(srcID, out Vertex<T> src))
                return false;

            if (!Vertices.TryGetValue(destID, out Vertex<T> dest))
                return false;

            if (!Edges[srcID].Contains(dest.ID))
                return false;

            Edges[srcID].Remove(dest.ID);
            return true;
        }

        internal Vertex<T> FindByValue(T value)
        {
            foreach (Vertex<T> vertex in Vertices.Values)
            {
                if (vertex.Data.Equals(value))
                    return vertex;
            }

            return null;
        }

        internal ISet<Vertex<T>> Subgraph(Graph<T> g)
        {
            // TODO: there are better subgraph finding algorithms
            ISet<Vertex<T>> remove = new HashSet<Vertex<T>>();
            foreach (Vertex<T> vertex in g)
            {
                Vertex<T> found;
                if ((found = FindByValue(vertex.Data)) == null)
                    return null;

                if (!Edges[vertex.ID].IsSubsetOf(Edges[found.ID]))
                    return null;

                remove.Add(found);
            }

            return remove;
        }

        internal static Graph<T> Intersect(Graph<T> left, Graph<T> right)
        {
            Graph<T> intersect = new Graph<T>();

            foreach (Vertex<T> vertex in left)
            {
                Vertex<T> found;
                if ((found = right.FindByValue(vertex.Data)) != null)
                    intersect.AddVertex(found);
            }

            return intersect;
        }

        public IEnumerator<Vertex<T>> GetEnumerator() => Vertices.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Vertices.Values.GetEnumerator();
    }
}