namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpNullCoalescingExpression : CSharpStatement
{
    public CSharpStatement Lhs { get; }
    public CSharpStatement Rhs { get; }

    public CSharpNullCoalescingExpression(CSharpStatement lhs, CSharpStatement rhs) : base(null)
    {
        Lhs = lhs;
        Rhs = rhs;
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}{Lhs.GetText(indentation).TrimStart()} ?? {Rhs.GetText(indentation).TrimStart()}";
    }
}