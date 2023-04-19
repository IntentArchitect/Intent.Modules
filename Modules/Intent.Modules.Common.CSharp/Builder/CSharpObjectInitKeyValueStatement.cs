using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpObjectInitKeyValueStatement : CSharpStatement, IHasCSharpStatements
{
    private readonly CSharpStatement _key;
    private readonly CSharpStatement _value;

    public CSharpObjectInitKeyValueStatement(CSharpStatement key, CSharpStatement value) : base(string.Empty)
    {
        _key = key;
        _value = value;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public override string GetText(string indentation)
    {
        return $@"{indentation}{{ {_key.GetText("")}, {_value.GetText("")} }}";
    }
}