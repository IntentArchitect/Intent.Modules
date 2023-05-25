namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpTryBlock : CSharpStatementBlock
{
    public CSharpTryBlock() : base("try")
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
    }
}