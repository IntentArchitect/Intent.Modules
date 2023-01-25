using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Java.Builder;

public class JavaFieldAssignmentStatement: JavaStatement
{
    private readonly string _lhs;
    private readonly string _rhs;

    public JavaFieldAssignmentStatement(string lhs, string rhs) : base(null)
    {
        _lhs = lhs.RemovePrefix("this.");
        _rhs = rhs;
    }

    // https://stackoverflow.com/a/47710/802755
    // public JavaFieldAssignmentStatement ThrowArgumentNullException()
    // {
    //     AfterAssignment = $" ?? throw new ArgumentNullException(nameof({_rhs}))";
    //     return this;
    // }

    public override string GetText(string indentation)

    {
        return $"{indentation}{(_lhs == _rhs ? "this." : "")}{_lhs} = {_rhs}{AfterAssignment};";
    }

    public string AfterAssignment { get; set; }
}