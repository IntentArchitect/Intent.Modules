using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Intent.Modules.Common.CSharp.AppStartup;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAccessMemberStatement : CSharpStatement
{
    private bool _withSemicolon = false;
    private bool _isConditional = false;
    public CSharpAccessMemberStatement(CSharpStatement expression, CSharpStatement memberName) : base($"{expression.ToString().TrimEnd()}.{memberName}")
    {
        Reference = expression;
        Member = memberName;
    }

    private CSharpStatement Reference { get; }
    private CSharpStatement Member { get; }

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

    public override string GetText(string indentation)
    {
        return $"{Reference.GetText(indentation).TrimEnd()}{(_isConditional ? "?." : ".")}{Member.GetText(indentation).Trim()}{(_withSemicolon ? ";" : string.Empty)}";
    }
}

public static class CSharpStatementBuilder
{

}

public class CSharpInvocationStatement : CSharpStatement, IHasCSharpStatements
{
    private bool _withSemicolon = true;
    private CSharpCodeSeparatorType _defaultArgumentSeparator = CSharpCodeSeparatorType.None;
    private CSharpClassMethod? _invokedMethod;


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
	internal CSharpInvocationStatement Invokes(CSharpClassMethod method)
	{
        _invokedMethod = method;
        return this;
	}

	public override string GetText(string indentation)
    {
        if (_invokedMethod?.IsAsync == true)
        {
            return AsyncAwareInvocationText(indentation);
        }
        return $"{RelativeIndentation}{Expression.GetText(indentation)}({GetArgumentsText(indentation)}){(_withSemicolon ? ";" : string.Empty)}";
    }

    private string AsyncAwareInvocationText(string indentation)
    {
        bool parentIsAsync = true;
        var parent = GetParentMethodorConstructor();
        string parentCancellationTokenName = null;
        if (parent != null)
        {
            switch (parent)
            {
                case CSharpConstructor ctor:
                    parentIsAsync = false;
                    break;
                case CSharpClassMethod method:
                    parentIsAsync = method.IsAsync;
                    var parameter = method.Parameters.FirstOrDefault(p => p.Type is "CancellationToken" or "CancellationToken?");
					if (parameter != null)
                    {
                        parentCancellationTokenName = parameter.Name;
					}
                    break;
			}
		}
        if (parentIsAsync)
        {
            var argumentCancellationToken = (CSharpStatement)parentCancellationTokenName;
			argumentCancellationToken.BeforeSeparator = _defaultArgumentSeparator;
			argumentCancellationToken.AfterSeparator = CSharpCodeSeparatorType.None;

			return $"{RelativeIndentation}await {Expression.GetText(indentation)}({GetArgumentsText(indentation, argumentCancellationToken)}){(_withSemicolon ? ";" : string.Empty)}";
		}
		else
        {
			return $"{RelativeIndentation}{Expression.GetText(indentation)}({GetArgumentsText(indentation)}).GetAwaiter().GetResult(){(_withSemicolon ? ";" : string.Empty)}";
		}
	}

    private IHasCSharpStatements? GetParentMethodorConstructor()
    {
        var current = Parent;
        while (current != null && current is not CSharpClassMethod or CSharpConstructor)
        {
            current = (current as CSharpStatement)?.Parent;
        }
        return current;
	}

	private string GetArgumentsText(string indentation, params CSharpStatement[] additionalArguments)
    {
        var additionalIndentation = GetAdditionalIndentationIfArgsOnNewLines();
		return Statements.Concat(additionalArguments ?? Enumerable.Empty<CSharpStatement>()).JoinCode(",", $"{indentation}{additionalIndentation}");
    }

    private string GetAdditionalIndentationIfArgsOnNewLines()
    {
		return Statements.All(x => x.BeforeSeparator == CSharpCodeSeparatorType.None)
            ? string.Empty
            : "    ";
    }
}