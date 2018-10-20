using LindenGen.Graph;
using Sprache;
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

    internal class ContinuingTerm
    {
        public ArrowType ArrowType { get; }
        public Symbol Term { get; }

        public ContinuingTerm(ArrowType type, Symbol term)
        {
            ArrowType = type;
            Term = term;
        }
    }

    internal class Line
    {

    }

    public class SymbolType
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public SymbolType(int id, string name)
        {
            ID = id;
            Name = name;
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
        private static int symbolTypeID = 0;

        private static readonly CommentParser commentParser = new CommentParser("#", "#!", "!#", "\n");
        
        // Parsing symbols
        private static readonly Parser<SymbolType> symbol =
            from name in Parse.Letter.AtLeastOnce().Text()
            from space in Parse.WhiteSpace.Many()
            from comment in commentParser.AnyComment.Many()
            from space2 in Parse.WhiteSpace.Many()
            select new SymbolType(symbolTypeID++, name);

        // Parsing axioms
        private static readonly Parser<Symbol> term =
            from type in Parse.Letter.AtLeastOnce().Text()
            from ident in Parse.Digit.Many().Text()
            from space in Parse.WhiteSpace.Many()
            select new Symbol(type, ident);

        private static Parser<ArrowType> Connector(string op, ArrowType type) => Parse.String(op).Token().Return(type);
        static readonly Parser<ArrowType> Strong = Connector("->", ArrowType.Weak);
        static readonly Parser<ArrowType> Weak = Connector("->>", ArrowType.Strong);
        static readonly Parser<ArrowType> Block = Connector("-o", ArrowType.Block);

        private static readonly Parser<ArrowType> arrowType =
            from type in Parse.String("->").Return(ArrowType.Weak)
                .Or(Parse.String("->>").Return(ArrowType.Strong))
                .Or(Parse.String("-o").Return(ArrowType.Block))
            select type;

        private static readonly Parser<ContinuingTerm> continuingTerm =
            from connector in Weak.Or(Strong).Or(Block)
            from space in Parse.WhiteSpace.Many()
            from next in term
            select new ContinuingTerm(connector, next);

        private static readonly Parser<Graph<Symbol>> graph =
            from first in term
            from following in continuingTerm.Many()
            from comment in commentParser.AnyComment.Many()
            from space in Parse.WhiteSpace.Many()
            select new Graph<Symbol>();

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
            select new Rule(rule.Left, rule.Right);

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