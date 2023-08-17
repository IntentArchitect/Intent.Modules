using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAssignmentStatement : CSharpStatement, IHasCSharpStatements
{
    private readonly CSharpStatement _lhs;
    private readonly CSharpStatement _rhs;

    public CSharpAssignmentStatement(CSharpStatement lhs, CSharpStatement rhs) : base(null)
    {
        _lhs = lhs;
        _rhs = rhs;
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public override string GetText(string indentation)
    {
        return $@"{indentation}{RelativeIndentation}{_lhs.GetText(indentation).TrimEnd()} = {_rhs.GetText(indentation).TrimStart()}{TrailingCharacter?.ToString() ?? ""}";
    }
}