using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAssignmentStatement : CSharpStatement, IHasCSharpStatements
{

    public CSharpAssignmentStatement(CSharpStatement lhs, CSharpStatement rhs) : base(null)
    {
        Lhs = lhs;
        Rhs = rhs;
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public CSharpStatement Lhs { get; set; }
    public CSharpStatement Rhs { get; set; }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public override string GetText(string indentation)
    {
        return $@"{RelativeIndentation}{Lhs.GetText(indentation).TrimEnd()} = {Rhs.GetText(indentation).TrimStart()}{TrailingCharacter?.ToString() ?? ""}";
    }
}