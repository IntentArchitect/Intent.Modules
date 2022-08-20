using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpConstructor
{
    private readonly IList<CSharpConstructorParameter> _parameters = new List<CSharpConstructorParameter>();
    private readonly IList<string> _statements = new List<string>();
    public CSharpClass Class { get; }
    public string AccessModifier { get; private set; } = "public ";
    public CSharpConstructor(CSharpClass @class)
    {
        Class = @class;
    }

    public CSharpConstructorParameter AddParameter(string type, string name)
    {
        var param = new CSharpConstructorParameter(type, name, this);
        _parameters.Add(param);
        return param;
    }

    public CSharpConstructor AddStatement(string statement)
    {
        _statements.Add(statement);
        return this;
    }

    public CSharpConstructor Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public CSharpConstructor Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public string ToString(string indentation)
    {
        return $@"{indentation}{AccessModifier}{Class.Name}({string.Join(", ", _parameters.Select(x => x.ToString()))})
{indentation}{{{(_statements.Any() ? $@"
{indentation}    {string.Join($@"
{indentation}    ", _statements)}" : string.Empty)}
{indentation}}}";
    }
}