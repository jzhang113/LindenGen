using LindenGen.Graph;
using System.Collections.Generic;

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
            ISet<Vertex<Symbol>> subvertices;
            if ((subvertices = g.Subgraph(Left)) == null)
                return false;

            // remove extra vertices
            // TODO: wrong behavior around dangling edges
            // this might be single push out behavior? idkkkkk
            List<int> remove = new List<int>();
            foreach (Vertex<Symbol> vertex in Left)
            {
                Vertex<Symbol> found;
                if ((found = Intersect.FindByValue(vertex.Data)) == null)
                    remove.Add(vertex.ID);
            }

            foreach (int id in remove)
            {
                g.RemoveVertex(id);
            }

            // remove edges in L-K
            foreach (Vertex<Symbol> vertex in Left)
            {
                foreach (int neighborID in Left.Edges[vertex.ID])
                {
                    g.RemoveEdge(vertex.ID, neighborID);
                }
            }

            // add new vertices
            foreach (Vertex<Symbol> vertex in Right)
            {
                Vertex<Symbol> found;
                if ((found = Intersect.FindByValue(vertex.Data)) == null)
                    g.AddVertex(vertex);
            }

            // add edges in R-K
            foreach (Vertex<Symbol> vertex in Right)
            {
                foreach (int neighborID in Right.Edges[vertex.ID])
                {
                    g.AddEdge(vertex.ID, neighborID);
                }
            }

            return true;
        }
    }
}