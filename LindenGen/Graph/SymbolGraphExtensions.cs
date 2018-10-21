using System.Collections.Generic;
using System.Linq;

namespace LindenGen.Graph
{
    internal static class SymbolGraphExtensions
    {
        internal static void Dump(this Graph<Symbol> g)
        {
            foreach (Vertex<Symbol> vertex in g)
            {
                System.Console.WriteLine(vertex.Data.Name);
                System.Console.Write("Neighbors: ");

                foreach (int neighborID in g.Edges[vertex.ID])
                {
                    System.Console.Write(g.Vertices[neighborID].Data.Name);
                    System.Console.Write(" ");
                }

                System.Console.WriteLine();
            }
        }

        internal static IEnumerable<Vertex<Symbol>> FindByType(this Graph<Symbol> g, SymbolType type)
        {
            foreach (Vertex<Symbol> vertex in g.Vertices.Values)
            {
                if (vertex.Data.Type.Equals(type))
                    yield return vertex;
            }
        }

        internal static IDictionary<int, int> FindMapping(this Graph<Symbol> g, Graph<Symbol> find)
        {
            foreach (Vertex<Symbol> vertex in g.Vertices.Values)
            {
                vertex.Seen = false;
            }

            foreach (Vertex<Symbol> vertex in find.Vertices.Values)
            {
                vertex.Seen = false;
            }

            IDictionary<int, int> matched = new Dictionary<int, int>();
            
            // pick a random point in find
            foreach (Vertex<Symbol> vertex in find.Vertices.Values)
            {
                if (vertex.Seen)
                    continue;

                FindOnGraph(g, find, matched, vertex, null);

                // If we can't find a vertex, there is no mapping
                if (matched.Count == 0)
                    return null;
            }

            // don't return an incomplete mapping
            if (matched.Count < find.Vertices.Count)
                return null;

            return matched;
        }

        private static void FindOnGraph(Graph<Symbol> g, Graph<Symbol> find, IDictionary<int, int> matched, Vertex<Symbol> curr, Vertex<Symbol> prev)
        {
            // If all of the values have been mapped, we are done
            if (find.Vertices.Values.All(x => x.Seen))
                return;

            // match it to the graph
            foreach (Vertex<Symbol> candidate in g.FindByType(curr.Data.Type).ToList())
            {
                if (candidate.Seen)
                    continue;

                if (prev != null && !g.Edges[prev.ID].Contains(candidate.ID))
                    continue;

                matched[curr.ID] = candidate.ID;
                candidate.Seen = true;
                curr.Seen = true;

                // follow the point in find
                foreach (int neighbor in find.Edges[curr.ID])
                {
                    Vertex<Symbol> newVertex = find.Vertices[neighbor];
                    if (newVertex.Seen) // probably need to check that our mapping is knit here
                        continue;

                    // newVertex.Seen = true;

                    FindOnGraph(g, find, matched, newVertex, candidate);
                }
            }

            // Otherwise, we must have had a bad assignment, so undo the latest assignment?
        }
    }
}
