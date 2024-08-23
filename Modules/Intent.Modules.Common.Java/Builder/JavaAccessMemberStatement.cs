using System;

namespace Intent.Modules.Common.Java.Builder;

public class JavaAccessMemberStatement : JavaStatement
{
    private bool _withSemicolon;
    private bool _isConditional;
    private bool _onNewLine;

    public JavaAccessMemberStatement(JavaStatement expression, JavaStatement memberName) : base($"{expression.ToString().TrimEnd()}.{memberName}")
    {
        Reference = expression;
        Member = memberName;
    }

    private JavaStatement Reference { get; }
    public JavaStatement Member { get; }

    public JavaAccessMemberStatement WithSemicolon()
    {
        _withSemicolon = true;
        return this;
    }

    public JavaAccessMemberStatement WithoutSemicolon()
    {
        _withSemicolon = false;
        return this;
    }

    /// <summary>
    /// Ensures that the member is accessed via the `?.` operator.
    /// </summary>
    /// <returns></returns>
    public JavaAccessMemberStatement IsConditional()
    {
        _isConditional = true;
        return this;
    }

    public JavaAccessMemberStatement OnNewLine()
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