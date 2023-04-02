namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpCatchBlock : CSharpStatementBlock
{
    public CSharpCatchBlock() : base($"catch")
    {
    }

    public CSharpCatchBlock(string exceptionType, string parameterName) : base($"catch ({exceptionType} {parameterName})")
    {
    }
}