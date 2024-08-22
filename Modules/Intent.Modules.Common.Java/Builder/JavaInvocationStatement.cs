using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public class JavaInvocationStatement : JavaStatement, IHasJavaStatements
{
    private bool _withSemicolon = true;
    private JavaCodeSeparatorType _defaultArgumentSeparator = JavaCodeSeparatorType.None;

	public JavaInvocationStatement(string invokable) : base(invokable)
    {
        Expression = new JavaStatement(invokable);
    }

	public JavaInvocationStatement(JavaStatement expression, string member) : base($"{expression.ToString().TrimEnd()}.{member}")
    {
        Expression = new JavaAccessMemberStatement(expression, member);
    }

    public JavaInvocationStatement(IJavaExpression expression) : base($"{expression.ToString().TrimEnd()}")
    {
        Expression = expression;
    }

    public IJavaExpression Expression { get; } 
    public IList<JavaStatement> Statements { get; } = new List<JavaStatement>();


	public JavaInvocationStatement AddArgument(JavaStatement argument, Action<JavaStatement> configure = null)
    {
        argument.Parent = this;
        Statements.Add(argument);
        argument.BeforeSeparator = _defaultArgumentSeparator;
        argument.AfterSeparator = JavaCodeSeparatorType.None;
        configure?.Invoke(argument);
        return this;
    }

    public JavaInvocationStatement AddArgument(JavaArgument argument, Action<JavaArgument> configure = null)
    {
        return AddArgument((JavaStatement)argument, configure != null ? x => configure((JavaArgument)x) : null);
    }

    public JavaInvocationStatement WithArgumentsOnNewLines()
    {
        foreach (var argument in Statements)
        {
            argument.BeforeSeparator = JavaCodeSeparatorType.NewLine;
        }

        _defaultArgumentSeparator = JavaCodeSeparatorType.NewLine;
        return this;
    }

    /// <summary>
    /// If this invocation statement is part of an invocation chain, calling this method
    /// will place the current invocation on a new line otherwise it remains on the current line.
    /// <br />
    /// Example:
    /// <code>
    /// new JavaStatement("service")
    ///   .AddInvocation("methodOne")
    ///   .AddInvocation("methodTwo", s => s.OnNewLine());
    /// </code>
    ///
    /// Will produce:
    /// <code>
    /// service.methodOne()
    ///     .methodTwo();
    /// </code>
    /// </summary>
    public JavaInvocationStatement OnNewLine()
    {
        (Expression as JavaAccessMemberStatement)?.OnNewLine();
        return this;
    }

    public JavaInvocationStatement WithoutSemicolon()
    {
        _withSemicolon = false;
        return this;
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
		return Statements.All(x => x.BeforeSeparator == JavaCodeSeparatorType.None)
            ? string.Empty
            : "    ";
    }
}