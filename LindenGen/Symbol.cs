using System.Collections.Generic;

namespace LindenGen
{
    public struct Symbol
    {
        public static int SymbolID = 0;

        public int ID { get; }
        public SymbolType Type { get; }
        public string Value { get; }
        public string Name => Type.Name + Value + "_" + ID;

        public Symbol(SymbolType type, string data)
        {
            Type = type;
            Value = data;
            ID = SymbolID++;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Symbol))
            {
                return false;
            }

            var symbol = (Symbol)obj;
            return EqualityComparer<SymbolType>.Default.Equals(Type, symbol.Type) &&
                   Value == symbol.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = 93709501;
            hashCode = hashCode * -1521134295 + EqualityComparer<SymbolType>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }
    }
}
