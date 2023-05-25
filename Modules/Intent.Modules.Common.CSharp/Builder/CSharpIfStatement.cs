namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpIfStatement : CSharpStatementBlock
{
    public CSharpIfStatement(string expression) : base(string.Empty)
    {
        Text = $"if ({expression?.TrimStart()})";
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
    }
}

public class CSharpElseIfStatement : CSharpStatementBlock
{
    public CSharpElseIfStatement(string expression) : base(string.Empty)
    {
        Text = $"else if ({expression?.TrimStart()})";
    }
}

public class CSharpElseStatement : CSharpStatementBlock
{
    public CSharpElseStatement() : base(string.Empty)
    {
        Text = $"else";
    }
}