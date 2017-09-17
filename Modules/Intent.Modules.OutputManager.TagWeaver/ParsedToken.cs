namespace Intent.Modules.OutputManager.TagWeaver
{
    public class ParsedToken
    {
        public ParsedToken(int index, int length, string name, string value )
        {
            Index = index;
            Length = length;
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
        public string Value { get; set; }
    }
}
