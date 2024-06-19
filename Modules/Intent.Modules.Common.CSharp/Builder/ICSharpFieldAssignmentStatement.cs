namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpFieldAssignmentStatement : ICSharpStatement
{
    public ICSharpFieldAssignmentStatement ThrowArgumentNullException();
    public string AfterAssignment { get; set; }
}