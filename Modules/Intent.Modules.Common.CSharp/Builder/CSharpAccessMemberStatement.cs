using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAccessMemberStatement : CSharpStatement
{
    private bool _withSemicolon;
    private bool _isConditional;
    private bool _onNewLine;
    
    public CSharpAccessMemberStatement(CSharpStatement expression, CSharpStatement memberName) : base($"{expression.ToString().TrimEnd()}.{memberName}")
    {
        Reference = expression;
        Member = memberName;
    }

    public CSharpAccessMemberStatement(CSharpStatement expression, CSharpStatement memberName, ICSharpReferenceable referenceable) : base($"{expression.ToString().TrimEnd()}.{memberName}", referenceable)
    {
        Reference = expression;
        Member = memberName;
    }

    private CSharpStatement Reference { get; }
    public CSharpStatement Member { get; }

    public CSharpAccessMemberStatement WithSemicolon()
    {
        _withSemicolon = true;
        return this;
    }

    public CSharpAccessMemberStatement WithoutSemicolon()
    {
        _withSemicolon = false;
        return this;
    }

    /// <summary>
    /// Ensures that the member is accessed via the `?.` operator.
    /// </summary>
    /// <returns></returns>
    public CSharpAccessMemberStatement IsConditional()
    {
        _isConditional = true;
        return this;
    }
    
    public CSharpAccessMemberStatement OnNewLine()
    {
        _onNewLine = true;
        return this;
    }

    public override string GetText(string indentation)
    {
        var newLineTxt = String.Empty;
        if (_onNewLine)
        {
            newLineTxt = Environment.NewLine + indentation + "    ";
        }
        return $"{Reference.GetText(indentation).TrimEnd()}{newLineTxt}{(_isConditional ? "?." : ".")}{Member.GetText(indentation).Trim()}{(_withSemicolon ? ";" : string.Empty)}";
    }
}