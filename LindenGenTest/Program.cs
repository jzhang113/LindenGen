using LindenGen;
using LindenGen.Graph;
using System;
using System.Diagnostics;
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
                    Console.WriteLine(axiom);
                }

                Console.WriteLine("\n== Rules ==");
                foreach (Rule rule in g.Rules)
                {
                    Console.WriteLine(rule);
                }

                //Generator gen = new Generator();
                //gen.Load(parse.Compile());
                //gen.Run();
            }

            System.Console.ReadLine();
        }
    }
}
