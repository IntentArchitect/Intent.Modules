using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Intent.Modules.Common.CSharp.AppStartup;

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

    /// <summary>
    /// If this invocation statement is part of an invocation chain, calling this method
    /// will place the current invocation on a new line otherwise it remains on the current line.
    /// <br />
    /// Example:
    /// <code>
    /// new CSharpStatement("service")
    ///   .AddInvocation("MethodOne")
    ///   .AddInvocation("MethodTwo", s => s.OnNewLine());
    /// </code>
    ///
    /// Will produce:
    /// <code>
    /// service.MethodOne()
    ///     .MethodTwo();
    /// </code>
    /// </summary>
    public CSharpInvocationStatement OnNewLine()
    {
        (Expression as CSharpAccessMemberStatement)?.OnNewLine();
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