using LindenGen.Graph;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LindenGen
{
    internal enum ArrowType
    {
        Weak,
        Strong,
        Block
    }

    internal class ContinuingTerm<T>
    {
        public ArrowType ArrowType { get; }
        public T Term { get; }

        public ContinuingTerm(ArrowType type, T term)
        {
            ArrowType = type;
            Term = term;
        }
    }

    public class Grammar
    {
        public ICollection<SymbolType> Symbols { get; }
        public ICollection<Graph<Symbol>> Axioms { get; }
        public ICollection<Rule> Rules { get; }

        public Grammar(List<SymbolType> symbols, List<Graph<Symbol>> axioms, List<Rule> rules)
        {
            Symbols = symbols;
            Axioms = axioms;
            Rules = rules;
        }
    }

    public static class Parser
    {
        private static readonly CommentParser commentParser = new CommentParser("#", "#!", "!#", "\n");
        
        // Parsing symbols
        private static readonly Parser<SymbolType> symbol =
            from name in Parse.Letter.AtLeastOnce().Text()
            from space in Parse.WhiteSpace.Many()
            from comment in commentParser.AnyComment.Many()
            from space2 in Parse.WhiteSpace.Many()
            select new SymbolType(SymbolTable.TypeID++, name);

        // Parsing axioms
        private static readonly Parser<Symbol> term =
            from type in Parse.Letter.AtLeastOnce().Text()
            from ident in Parse.Digit.Many().Text()
            from space in Parse.WhiteSpace.Many()
            select new Symbol(SymbolTable.Symbols[type], ident);

        private static readonly Parser<ArrowType> arrowType =
            from type in Parse.String("->>").Return(ArrowType.Strong)
                .Or(Parse.String("->").Return(ArrowType.Weak))
                .Or(Parse.String("-o").Return(ArrowType.Block))
            select type;

        private static readonly Parser<ContinuingTerm<Symbol>> continuingTerm =
            from connector in arrowType
            from space in Parse.WhiteSpace.Many()
            from next in term
            select new ContinuingTerm<Symbol>(connector, next);

        private static readonly Parser<Graph<Symbol>> graph =
            from first in term
            from following in continuingTerm.Many()
            from comment in commentParser.AnyComment.Many()
            from space in Parse.WhiteSpace.Many()
            select new Graph<Symbol>(first, following);

        // Parsing rules
        private static readonly Parser<Rule> rule =
            from left in graph
            from arrow in Parse.String("=>")
            from space in Parse.WhiteSpace.Many()
            from right in graph
            from comment2 in commentParser.AnyComment.Many()
            from space2 in Parse.WhiteSpace.Many()
            select new Rule(left, right);

        private static readonly Parser<Graph<Symbol>> continuingGraph =
            from delimiter in Parse.Char(',')
            from space in Parse.WhiteSpace.Many()
            from next in graph
            select next;

        private static readonly Parser<Rule> multirule =
            from rule in rule
            from following in continuingGraph.Many()
            select new Rule(rule.Left, following.Aggregate(rule.Right, (current, append) =>
            {
                IDictionary<int, int> translation = new Dictionary<int, int>();
                foreach (Vertex<Symbol> vertex in append.Vertices.Values)
                {
                    int newID = current.AddVertex(vertex.Data).ID;
                    translation.Add(vertex.ID, newID);
                }

                foreach (var kvp in append.Edges)
                {
                    int newKey = translation[kvp.Key];
                    foreach (int neighbor in kvp.Value)
                    {
                        current.AddEdge(newKey, translation[neighbor]);
                    }
                }

                return current;
            }));

        private static readonly Parser<Grammar> grammar =
            from leading in Parse.WhiteSpace.Many()
            from headerSymbols in Parse.String("!Symbols")
            from space in Parse.WhiteSpace.Many()
            from comment in commentParser.AnyComment.Many()
            from space2 in Parse.WhiteSpace.Many()
            from symbolset in symbol.AtLeastOnce()
            from headerAxioms in Parse.String("!Axioms")
            from space3 in Parse.WhiteSpace.Many()
            from axiomset in graph.AtLeastOnce()
            from headerRules in Parse.String("!Rules")
            from space4 in Parse.WhiteSpace.Many()
            from comment2 in commentParser.AnyComment.Many()
            from space5 in Parse.WhiteSpace.Many()
            from ruleset in multirule.AtLeastOnce()
            from trailing in Parse.WhiteSpace.Many().End()
            select new Grammar(symbolset.ToList(), axiomset.ToList(), ruleset.ToList());

        public static Grammar ParseText(string text)
        {
            return grammar.Parse(text);
        }
    }
}