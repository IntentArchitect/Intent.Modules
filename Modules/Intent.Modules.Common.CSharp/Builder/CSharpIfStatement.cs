namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpIfStatement : CSharpStatementBlock
{
    public CSharpIfStatement(string expression) : base(string.Empty)
    {
        Text = $"if ({expression?.TrimStart()})";
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
    }
}