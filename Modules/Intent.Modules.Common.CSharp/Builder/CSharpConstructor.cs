using System;
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
    public CSharpConstructorCall ConstructorCall { get; private set; }
    public CSharpConstructor(CSharpClass @class)
    {
        Class = @class;
    }

    public CSharpConstructor AddParameter(string type, string name, Action<CSharpConstructorParameter> configure = null)
    {
        var param = new CSharpConstructorParameter(type, name, this);
        _parameters.Add(param);
        configure?.Invoke(param);
        return this;
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

    public CSharpConstructor CallsBase(Action<CSharpConstructorCall> configure = null)
    {
        ConstructorCall = CSharpConstructorCall.Base();
        configure?.Invoke(ConstructorCall);
        return this;
    }

    public CSharpConstructor CallsThis(Action<CSharpConstructorCall> configure = null)
    {
        ConstructorCall = CSharpConstructorCall.This();
        configure?.Invoke(ConstructorCall);
        return this;
    }

    public string ToString(string indentation)
    {
        return $@"{indentation}{AccessModifier}{Class.Name}({string.Join(", ", _parameters.Select(x => x.ToString()))}){ConstructorCall?.ToString() ?? string.Empty}
{indentation}{{{(_statements.Any() ? $@"
{indentation}    {string.Join($@"
{indentation}    ", _statements)}" : string.Empty)}
{indentation}}}";
    }
}

public class CSharpConstructorCall
{
    private readonly string _type;
    public IList<string> Arguments { get; } = new List<string>();

    private CSharpConstructorCall(string type)
    {
        _type = type;
    }

    public static CSharpConstructorCall Base()
    {
        return new CSharpConstructorCall("base");
    }

    public static CSharpConstructorCall This()
    {
        return new CSharpConstructorCall("this");
    }

    public CSharpConstructorCall AddArgument(string name)
    {
        Arguments.Add(name);
        return this;
    }

    public override string ToString()
    {
        if (Arguments.Any())
        {
            return $" : {_type}({string.Join(", ", Arguments)})";
        }

        return string.Empty;
    }
}
