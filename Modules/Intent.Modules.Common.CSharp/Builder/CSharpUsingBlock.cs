namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpUsingBlock : CSharpStatementBlock
{
    public CSharpUsingBlock(string expression) : base(string.Empty)
    {
        Text = $"using ({expression?.TrimStart()})";
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
    }
}