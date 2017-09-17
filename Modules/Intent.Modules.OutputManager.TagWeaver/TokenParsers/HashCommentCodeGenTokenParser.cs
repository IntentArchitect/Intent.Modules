namespace Intent.Modules.OutputManager.TagWeaver.TokenParsers
{
    public class HashCommentCodeGenTokenParser : TokenParser
    {
        private const string CODE_GEN_PATTERN = @"[ \f\t\v]*\#cg\[(.*?)\][ \f\t\v]*\r\n";

        public HashCommentCodeGenTokenParser()
            : base("CodeGen", CODE_GEN_PATTERN)
        {
        }

        public override ParsedToken Parse(int index, int length, string value)
        {
            return new HashCommentCodeGenParsedToken(index, length, Name, value);
        }
    }
}
