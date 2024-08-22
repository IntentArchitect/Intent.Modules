using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpReturnStatement : CSharpStatement
{
    public CSharpReturnStatement(CSharpStatement returnStatement)
    {
        ArgumentNullException.ThrowIfNull(returnStatement);
        ReturnStatement = returnStatement;
        TrailingCharacter = ';';
    }
    
    public CSharpStatement ReturnStatement { get; }

    public CSharpReturnStatement WithoutSemicolon()
    {
        TrailingCharacter = null;
        return this;
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}return {ReturnStatement.GetText(indentation).TrimStart()}{(TrailingCharacter != null ? TrailingCharacter : "")}";
    }
}