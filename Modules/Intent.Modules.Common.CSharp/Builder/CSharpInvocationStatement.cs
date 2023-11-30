using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.AppStartup;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpAccessMemberStatement : CSharpStatement
{
    private bool _withSemicolon = false;
    public CSharpAccessMemberStatement(CSharpStatement reference, CSharpStatement memberName) : base($"{reference.ToString().TrimEnd()}.{memberName}")
    {
        Reference = reference;
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

    public override string GetText(string indentation)
    {
        return $"{Reference.GetText(indentation).TrimEnd()}.{Member}{(_withSemicolon ? ";" : string.Empty)}";
    }
}

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