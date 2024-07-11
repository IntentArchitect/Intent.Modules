using System;

namespace Intent.Modules.Common.CSharp.Builder;

// https://sharplab.io/#v2:C4LghgzsA0AmIGoA+ABATARgLACgUGYACdQgYUIG9dCbiiUAWQgWQAoBKS62ngNzABOhAMYB7AHaxCAXkIBxAKbBSE2BwDc3HjX5CBCiAFcANsBkjVhAPyEARMAGGFtwiDsAzMMYjPNOHgC+WoQARqKixvJKKpIc0gB8Dk5+AUA=

public class CSharpConditionalExpressionStatement : CSharpStatement
{
    public CSharpConditionalExpressionStatement(CSharpStatement condition, CSharpStatement whenTrue, CSharpStatement whenFalse)
    {
        ArgumentNullException.ThrowIfNull(condition);
        ArgumentNullException.ThrowIfNull(whenTrue);
        ArgumentNullException.ThrowIfNull(whenFalse);
        
        Condition = condition;
        WhenTrue = whenTrue;
        WhenFalse = whenFalse;
    }
    
    public ICSharpExpression Condition { get; }
    public ICSharpExpression WhenTrue { get; }
    public ICSharpExpression WhenFalse { get; }

    public override string GetText(string indentation)
    {
        var text = $"{RelativeIndentation}{Condition.GetText(indentation)} ? {WhenTrue.GetText(string.Empty)} : {WhenFalse.GetText(string.Empty)}";
        if (text.Length <= 120)
        {
            return text;
        }
        
        return $"""
                {indentation}{RelativeIndentation}{Condition.GetText(indentation).TrimStart()}
                {indentation}{RelativeIndentation}    ? {WhenTrue.GetText($"{indentation}    ").TrimStart()}
                {indentation}{RelativeIndentation}    : {WhenFalse.GetText($"{indentation}    ").TrimStart()}
                """;
    }
}