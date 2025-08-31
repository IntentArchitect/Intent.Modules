using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAssignmentStatement : CSharpStatement, IHasCSharpStatements
{
    public CSharpAssignmentStatement(CSharpStatement lhs, CSharpStatement rhs) : base(null)
    {
        Lhs = lhs;
        Rhs = rhs;
        Rhs.Parent = this;
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public CSharpStatement Lhs { get; set; }
    public CSharpStatement Rhs { get; set; }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public override string GetText(string indentation)
    {
        var rhs = Rhs.GetText(indentation).TrimStart();
        return $@"{RelativeIndentation}{Lhs.GetText(indentation).TrimEnd()} = {rhs}{(TrailingCharacter != null && !rhs.EndsWith(TrailingCharacter.Value) ? TrailingCharacter.Value : "")}";
    }

    bool IHasCSharpStatementsActual.IsCodeBlock => Parent?.IsCodeBlock ?? false;
}