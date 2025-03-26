using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpObjectInitStatement : CSharpStatement, IHasCSharpStatements
{
    private string _lhs;
    private CSharpStatement _rhs;

    public CSharpObjectInitStatement(string lhs, CSharpStatement rhs) : base(null)
    {
        _lhs = lhs;
        _rhs = rhs;
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public string LeftHandSide => _lhs;

    public CSharpObjectInitStatement WithLeftHandSide(string lhs)
    {
        _lhs = lhs; 
        return this;
    }

    public CSharpStatement RightHandSide => _rhs;

    public CSharpObjectInitStatement WithRightHandSide(CSharpStatement rhs)
    {
        _rhs = rhs;
        return this;
    }

    public override string GetText(string indentation)
    {
        var rhs = _rhs.GetText(indentation).TrimStart();
        if (rhs.StartsWith('{'))
        {
            rhs = $"{Environment.NewLine}{indentation}{rhs}";
        }

        return $@"{indentation}{RelativeIndentation}{_lhs} = {rhs}";
    }

    bool IHasCSharpStatementsActual.IsCodeBlock => false;
}