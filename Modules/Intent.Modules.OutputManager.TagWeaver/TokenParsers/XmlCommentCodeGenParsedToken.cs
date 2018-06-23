namespace Intent.Modules.OutputManager.TagWeaver.TokenParsers
{
    public class XmlCommentCodeGenParsedToken : TagToken
    {

        public XmlCommentCodeGenParsedToken(int index, int length, string name, string value)
            : base(index, length, name, value)
        {
            string parse = value.Substring(value.IndexOf("<!--IntentManaged[") + 7);

            int start = 0;


            int endOfTag = parse.IndexOf("]");
            Identifier = parse.Substring(start, endOfTag - start + 1);
        }
    }
}

