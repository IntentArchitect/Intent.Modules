namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpIfStatement : CSharpStatementBlock
{
    public CSharpIfStatement(string expression) : base(string.Empty)
    {
        Text = $"if ({expression})";
    }
}

public class CSharpElseIfStatement : CSharpStatementBlock
{
    public CSharpElseIfStatement(string expression) : base(string.Empty)
    {
        Text = $"else if ({expression})";
    }
}

public class CSharpElseStatement : CSharpStatementBlock
{
    public CSharpElseStatement() : base(string.Empty)
    {
        Text = $"else";
    }
}