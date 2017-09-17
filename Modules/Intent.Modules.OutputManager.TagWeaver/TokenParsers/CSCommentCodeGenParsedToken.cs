namespace Intent.Modules.OutputManager.TagWeaver.TokenParsers
{
    public class CSCommentCodeGenParsedToken : TagToken
    {

        public CSCommentCodeGenParsedToken(int index, int length, string name, string value)
            : base(index, length, name, value)
        {
            string parse = value.Substring(value.IndexOf("//IntentManaged[") + "//IntentManaged[".Length);

            int start = 0;
            int endOfTag = parse.IndexOf("]");
            Identifier = parse.Substring(start, endOfTag - start);
        }
    }
}
