namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpWhileStatement : CSharpStatementBlock
{
    public CSharpWhileStatement(string expression)
    {
        Text = $"while ({expression})";
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
    }
}