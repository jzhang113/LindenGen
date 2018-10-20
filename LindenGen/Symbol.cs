namespace LindenGen
{
    public struct Symbol
    {
        public string Type { get; }
        public string Value { get; }

        public Symbol(string type, string data)
        {
            Type = type;
            Value = data;
        }
    }
}
