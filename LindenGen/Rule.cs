using LindenGen.Graph;
using System.Collections.Generic;
using System.Linq;

namespace LindenGen
{
    public class Rule
    {
        public Graph<Symbol> Left { get; }
        public Graph<Symbol> Right { get; }

        private Graph<Symbol> Intersect { get; }

        public Rule(Graph<Symbol> left, Graph<Symbol> right)
        {
            Left = left;
            Right = right;
            Intersect = Graph<Symbol>.Intersect(left, right);
        }

        // double pushout?
        public bool Apply(Graph<Symbol> g)
        {
            // check if we can apply the rule
            IDictionary<int, int> leftMapping;
            if ((leftMapping = g.FindMapping(Left)) == null)
                return false;

            IDictionary<int, int> rightMapping = MatchSymbols(leftMapping);

            // remove extra vertices
            // TODO: wrong behavior around dangling edges
            // this might be single push out behavior? idkkkkk
            foreach (Vertex<Symbol> vertex in Left)
            {
                List<Vertex<Symbol>> exist = Intersect.FindByType(vertex.Data.Type).ToList();
                if (exist.Count == 0)
                {
                    g.RemoveVertex(leftMapping[vertex.ID]);
                }
            }

            // remove edges in L-K
            foreach (Vertex<Symbol> vertex in Left)
            {
                foreach (int neighborID in Left.Edges[vertex.ID])
                {
                    g.RemoveEdge(leftMapping[vertex.ID], leftMapping[neighborID]);
                }
            }

            // add new vertices
            foreach (Vertex<Symbol> vertex in Right)
            {
                if (Intersect.FindByValue(vertex.Data) == null)
                {
                    g.AddVertex(vertex);
                }
            }

            // add edges in R-K
            foreach (Vertex<Symbol> vertex in Right)
            {
                foreach (int neighborID in Right.Edges[vertex.ID])
                {
                    g.AddEdge(rightMapping[vertex.ID], rightMapping[neighborID]);
                }
            }

            return true;
        }

        // Map right side of rules to corresponding elements to the mappings of the left side
        private IDictionary<int, int> MatchSymbols(IDictionary<int, int> leftMapping)
        {
            IDictionary<int, int> rightMapping = new Dictionary<int, int>();
            foreach (Vertex<Symbol> vertex in Right.Vertices.Values)
            {
                Vertex<Symbol> leftMatch;
                if ((leftMatch = Left.FindByValue(vertex.Data)) != null)
                    rightMapping.Add(vertex.ID, leftMapping[leftMatch.ID]);
                else
                    rightMapping.Add(vertex.ID, vertex.ID); // map new elements to themselves
            }

            return rightMapping;
        }
    }
}