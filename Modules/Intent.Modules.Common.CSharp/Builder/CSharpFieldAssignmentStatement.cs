using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpFieldAssignmentStatement : CSharpStatement
{
    private readonly string _lhs;
    private readonly string _rhs;

    public CSharpFieldAssignmentStatement(string lhs, string rhs) : base(null)
    {
        _lhs = lhs.RemovePrefix("this.");
        _rhs = rhs;
    }

    public CSharpFieldAssignmentStatement ThrowArgumentNullException()
    {
        AfterAssignment = $" ?? throw new ArgumentNullException(nameof({_rhs}))";
        return this;
    }

    public override string GetText(string indentation)
    {
        return $"{(_lhs == _rhs ? "this." : "")}{_lhs} = {_rhs}{AfterAssignment};";
    }

    public string AfterAssignment { get; set; }
}