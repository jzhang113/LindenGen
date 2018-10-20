using LindenGen;
using LindenGen.Graph;
using System;
using System.IO;

namespace LindenGenTest
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("grammar.txt"))
            {
                string text = sr.ReadToEnd();
                Grammar g = Parser.ParseText(text);

                Console.WriteLine("== Symbols ==");
                foreach (SymbolType type in g.Symbols)
                {
                    Console.WriteLine(type.Name);
                }

                Console.WriteLine("\n== Axioms ==");
                foreach (Graph<Symbol> axiom in g.Axioms)
                {
                    Dump(axiom);
                }

                Console.WriteLine("\n== Rules ==");
                foreach (Rule rule in g.Rules)
                {
                    Dump(rule.Left);
                    Dump(rule.Right);
                }

                //Generator gen = new Generator();
                //gen.Load(parse.Compile());
                //gen.Run();
            }

            System.Console.ReadLine();
        }

        private static void Dump(Graph<Symbol> g)
        {
            foreach (Vertex<Symbol> vertex in g)
            {
                Console.WriteLine(vertex.Data.Name);
                Console.Write("Neighbors: ");

                foreach (int neighborID in g.Edges[vertex.ID])
                {
                    Console.Write(g.Vertices[neighborID].Data.Name);
                    Console.Write(" ");
                }

                Console.WriteLine();
            }
        }
    }
}
