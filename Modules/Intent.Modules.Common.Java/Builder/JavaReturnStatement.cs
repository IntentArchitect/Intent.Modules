using System;

namespace Intent.Modules.Common.Java.Builder;

public class JavaReturnStatement : JavaStatement
{
    public JavaReturnStatement(JavaStatement returnStatement) : base("")
    {
        ArgumentNullException.ThrowIfNull(returnStatement);
        ReturnStatement = returnStatement;
    }
    
    public JavaStatement ReturnStatement { get; }

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}return {ReturnStatement.GetText(indentation).TrimStart()};";
    }
}