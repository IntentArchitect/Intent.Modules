namespace Intent.Modules.OutputManager.TagWeaver.TokenParsers
{
    public class HashCommentCodeGenParsedToken : TagToken
    {

        public HashCommentCodeGenParsedToken(int index, int length, string name, string value)
            : base(index, length, name, value)
        {
            string parse = value.Substring(value.IndexOf("#cg[") + 4);

            int start = 0;
            int endOfTag = parse.IndexOf("]");
            Identifier = parse.Substring(start, endOfTag - start);
        }
    }
}
