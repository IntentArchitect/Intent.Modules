namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpElseIfStatement : CSharpStatementBlock
{
    public CSharpElseIfStatement(string expression) : base(string.Empty)
    {
        Text = $"else if ({expression?.TrimStart()})";
    }
}