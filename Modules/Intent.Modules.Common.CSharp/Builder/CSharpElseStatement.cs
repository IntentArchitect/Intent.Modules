namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpElseStatement : CSharpStatementBlock
{
    public CSharpElseStatement() : base(string.Empty)
    {
        Text = $"else";
    }
}