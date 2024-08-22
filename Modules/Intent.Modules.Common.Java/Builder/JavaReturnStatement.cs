using System;

namespace Intent.Modules.Common.Java.Builder;

public class JavaReturnStatement : JavaStatement
{
    public JavaReturnStatement(JavaStatement returnStatement) : base("")
    {
        ArgumentNullException.ThrowIfNull(returnStatement);
        ReturnStatement = returnStatement;
        TrailingCharacter = ';';
    }
    
    public JavaStatement ReturnStatement { get; }

    public JavaReturnStatement WithoutSemicolon()
    {
        TrailingCharacter = null;
        return this;
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}return {ReturnStatement.GetText(indentation).TrimStart()}{(TrailingCharacter != null ? TrailingCharacter : "")}";
    }
}