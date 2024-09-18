using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpObjectInitStatement : CSharpStatement, IHasCSharpStatements
{
    private readonly string _lhs;
    private readonly CSharpStatement _rhs;

    public CSharpObjectInitStatement(string lhs, CSharpStatement rhs) : base(null)
    {
        _lhs = lhs;
        _rhs = rhs;
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public override string GetText(string indentation)
    {
        return $@"{indentation}{RelativeIndentation}{_lhs} = {_rhs.GetText(indentation).TrimStart()}";
    }

    bool IHasCSharpStatementsActual.IsCodeBlock => false;
}