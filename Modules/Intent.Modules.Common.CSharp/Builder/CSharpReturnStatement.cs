using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpReturnStatement : CSharpStatement
{
    public CSharpReturnStatement(CSharpStatement returnStatement)
    {
        ArgumentNullException.ThrowIfNull(returnStatement);
        ReturnStatement = returnStatement;
    }
    
    public CSharpStatement ReturnStatement { get; }

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}return {ReturnStatement.GetText(indentation).TrimStart()};";
    }
}