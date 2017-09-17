namespace Intent.Modules.OutputManager.TagWeaver.TokenParsers
{
    public class CSCommentCodeGenTokenParser : TokenParser
    {
        private const string CODE_GEN_PATTERN = @"[ \f\t\v]*//IntentManaged\[(.*?)\][ \f\t\v]*\r\n";

        public CSCommentCodeGenTokenParser()
            : base("CodeGen", CODE_GEN_PATTERN)
        {
        }

        public override ParsedToken Parse(int index, int length, string value)
        {
            return new CSCommentCodeGenParsedToken(index, length, Name, value);
        }
    }
}
