using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.AppStartup;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInvocationStatement : CSharpStatement, IHasCSharpStatements
{
    private bool _withSemicolon = true;
    private CSharpCodeSeparatorType _defaultArgumentSeparator = CSharpCodeSeparatorType.None;

    public CSharpInvocationStatement(string invocation) : base(invocation)
    {
    }

    public CSharpInvocationStatement(CSharpStatement reference, string methodName) : base($"{reference.ToString().TrimEnd()}.{methodName}")
    {
    }

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

    public override string GetText(string indentation)
    {
        return $"{indentation}{RelativeIndentation}{Text}({GetArgumentsText(indentation)}){(_withSemicolon ? ";" : string.Empty)}";
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