using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptFieldAssignmentStatement : TypescriptStatement
{
    private readonly string _lhs;
    private readonly string _rhs;

    public TypescriptFieldAssignmentStatement(string lhs, string rhs) : base(null)
    {
        _lhs = lhs;
        _rhs = rhs;
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}{(_lhs == _rhs ? "this." : "")}{_lhs} = {_rhs};";
    }
}