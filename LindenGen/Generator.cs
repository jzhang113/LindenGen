using LindenGen.Graph;
using System.Collections.Generic;

namespace LindenGen
{
    public class Generator
    {
        private IList<Symbol> Symbols { get; }
        private IList<Rule> Rules { get; }
        private Graph<Symbol> Graph { get; }

        public Generator(Graph<Symbol> start)
        {
            Symbols = new List<Symbol>();
            Rules = new List<Rule>();
            Graph = start;
        }

        public void AddSymbol(Symbol symbol)
        {
            if (!Symbols.Contains(symbol))
                Symbols.Add(symbol);
        }

        public void AddRule(Rule rule)
        {
            if (!Rules.Contains(rule))
                Rules.Add(rule);
        }

        public Graph<Symbol> Run()
        {
            System.Console.WriteLine("\n== Start ==");
            Graph.Dump();

            foreach (Rule rule in Rules)
            {
                if (rule.Apply(Graph))
                {
                    System.Console.WriteLine("== Rewrite ==");
                    Graph.Dump();
                }
            }

            return Graph;
        }
    }
}
