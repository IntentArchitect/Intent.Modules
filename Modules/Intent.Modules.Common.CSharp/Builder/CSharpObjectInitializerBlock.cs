using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpObjectInitializerBlock : CSharpStatement, IHasCSharpStatements
{
    private readonly CSharpStatement _initialization;
    private bool _withSemicolon;
    
    public CSharpObjectInitializerBlock(string initialization) : base(initialization)
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }

    public CSharpObjectInitializerBlock(CSharpStatement initialization) : base("")
    {
        _initialization = initialization;
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public new CSharpObjectInitializerBlock WithSemicolon()
    {
        _withSemicolon = true;
        return this;
    }

    public CSharpObjectInitializerBlock AddInitStatement(string lhs, CSharpStatement rhs, Action<CSharpStatement> configureRhs = null)
    {
        rhs.Parent = this;
        Statements.Add(new CSharpObjectInitStatement(lhs, rhs));
        configureRhs?.Invoke(rhs);
        return this;
    }

    public CSharpObjectInitializerBlock AddKeyAndValue(CSharpStatement key, CSharpStatement value)
    {
        Statements.Add(new CSharpObjectInitKeyValueStatement(key, value));
        return this;
    }

    /// <inheritdoc/>
    public override string GetText(string indentation)
    {
        var sb = new StringBuilder();

        var initialization = _initialization?.GetText(indentation) ?? base.GetText(indentation);
        if (!string.IsNullOrWhiteSpace(initialization))
        {
            sb.AppendLine(initialization);
        }

        sb.Append(indentation);
        sb.Append(RelativeIndentation);
        sb.Append('{');
        sb.AppendLine(Statements.JoinCode(",", $"{indentation}{RelativeIndentation}    "));
        sb.Append(indentation);
        sb.Append(RelativeIndentation);
        sb.Append('}');

        if (_withSemicolon)
        {
            sb.Append(';');
        }

        return sb.ToString();
    }

    bool IHasCSharpStatementsActual.IsCodeBlock => false;
}