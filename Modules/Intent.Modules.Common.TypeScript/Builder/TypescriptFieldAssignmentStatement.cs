using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptFieldAssignmentStatement : TypescriptStatement
{
    private readonly string _lhs;
    private readonly string _rhs;

    public TypescriptFieldAssignmentStatement(string lhs, string rhs) : base(null)
    {
        _lhs = lhs.RemovePrefix("this.");
        _rhs = rhs;
    }

    public TypescriptFieldAssignmentStatement ThrowArgumentNullException()
    {
        AfterAssignment = $" ?? throw new ArgumentNullException(nameof({_rhs}))";
        return this;
    }

    public override string GetText(string indentation)

    {
        return $"{indentation}{(_lhs == _rhs ? "this." : "")}{_lhs} = {_rhs}{AfterAssignment};";
    }

    public string AfterAssignment { get; set; }
}