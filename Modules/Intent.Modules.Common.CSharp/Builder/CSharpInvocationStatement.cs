using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInvocationStatement : CSharpStatement, IHasCSharpStatements
{
    private bool _withSemicolon = true;
    private CSharpCodeSeparatorType _defaultArgumentSeparator = CSharpCodeSeparatorType.None;

    public CSharpInvocationStatement(string invokable) : base(invokable)
    {
        Expression = new CSharpStatement(invokable);
    }

    public CSharpInvocationStatement(CSharpStatement expression, string member) : base($"{expression.ToString().TrimEnd()}.{member}")
    {
        Expression = new CSharpAccessMemberStatement(expression, member);
    }

    public CSharpInvocationStatement(ICSharpExpression expression) : base($"{expression.ToString().TrimEnd()}")
    {
        Expression = expression;
    }

    public ICSharpExpression Expression { get; }
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();


    public CSharpInvocationStatement AddArgument(CSharpStatement argument, Action<CSharpStatement> configure = null)
    {
        argument.Parent = this;
        Statements.Add(argument);
        argument.BeforeSeparator = _defaultArgumentSeparator;
        argument.AfterSeparator = CSharpCodeSeparatorType.None;
        configure?.Invoke(argument);
        return this;
    }

    public CSharpInvocationStatement AddArgument(CSharpArgument argument, Action<CSharpArgument> configure = null)
    {
        return AddArgument((CSharpStatement)argument, configure != null ? x => configure((CSharpArgument)x) : null);
    }

    public CSharpInvocationStatement WithArgumentsOnNewLines()
    {
        foreach (var argument in Statements)
        {
            argument.BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        }

        _defaultArgumentSeparator = CSharpCodeSeparatorType.NewLine;
        return this;
    }

    public CSharpInvocationStatement WithoutSemicolon()
    {
        _withSemicolon = false;
        return this;
    }

    public bool IsAsyncInvocation()
    {
        return Expression.Reference is ICSharpMethodDeclaration method && (method.IsAsync || method.ReturnType?.GetText("").Contains("Task") == true);
    }

    public override string GetText(string indentation)
    {
        return $"{RelativeIndentation}{Expression.GetText(indentation)}({GetArgumentsText(indentation)}){(_withSemicolon ? ";" : string.Empty)}";
    }

    private string GetArgumentsText(string indentation)
    {
        var additionalIndentation = GetAdditionalIndentationIfArgsOnNewLines();
        return Statements.JoinCode(",", $"{indentation}{additionalIndentation}");
    }

    private string GetAdditionalIndentationIfArgsOnNewLines()
    {
        return Statements.All(x => x.BeforeSeparator == CSharpCodeSeparatorType.None)
            ? string.Empty
            : "    ";
    }
}