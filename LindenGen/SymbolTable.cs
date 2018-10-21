using System.Collections.Generic;

namespace LindenGen
{
    public struct SymbolType
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public SymbolType(int id, string name)
        {
            ID = id;
            Name = name;
            SymbolTable.Symbols.Add(name, this);
        }
    }

    internal static class SymbolTable
    {
        public static int TypeID = 0;
        public static IDictionary<string, SymbolType> Symbols = new Dictionary<string, SymbolType>();

        public static void Reset()
        {
            TypeID = 0;
            Symbols.Clear();
        }
    }
}
