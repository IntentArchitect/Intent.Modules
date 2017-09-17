namespace Intent.Modules.OutputManager.TagWeaver
{
    public class TagToken : ParsedToken
    {
        public string Identifier { get; protected set; }

        public TagToken(int index, int length, string name, string value) 
            : base(index, length, name, value)
        {

        }
    }
}
