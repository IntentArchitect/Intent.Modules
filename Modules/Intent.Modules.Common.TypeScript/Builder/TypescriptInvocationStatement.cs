using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptInvocationStatement : TypescriptStatement, IHasTypescriptStatements
{
    private TypescriptCodeSeparatorType _defaultArgumentSeparator = TypescriptCodeSeparatorType.None;

    public TypescriptInvocationStatement(string invocation) : base(invocation)
    {
    }

    public List<TypescriptStatement> Statements { get; } = new();

    public TypescriptInvocationStatement AddArgument(TypescriptStatement argument, Action<TypescriptStatement> configure = null)
    {
        argument.Parent = this;
        Statements.Add(argument);
        argument.BeforeSeparator = _defaultArgumentSeparator;
        argument.AfterSeparator = TypescriptCodeSeparatorType.None;
        configure?.Invoke(argument);
        return this;
    }

    public TypescriptInvocationStatement WithArgumentsOnNewLines()
    {
        foreach (var argument in Statements)
        {
            argument.BeforeSeparator = TypescriptCodeSeparatorType.NewLine;
        }

        _defaultArgumentSeparator = TypescriptCodeSeparatorType.NewLine;
        return this;
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}{Text}({GetArgumentsText(indentation)});";
    }

    private string GetArgumentsText(string indentation)
    {
        return Statements.JoinCode(",", $"{indentation}    ");
    }
}