using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpConstructor
{
    public CSharpClass Class { get; }
    public string AccessModifier { get; private set; } = "public ";
    public CSharpConstructorCall ConstructorCall { get; private set; }
    private IList<CSharpConstructorParameter> Parameters { get; } = new List<CSharpConstructorParameter>();
    private IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public CSharpConstructor(CSharpClass @class)
    {
        Class = @class;
    }

    public CSharpConstructor AddParameter(string type, string name, Action<CSharpConstructorParameter> configure = null)
    {
        var param = new CSharpConstructorParameter(type, name, this);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpConstructor AddStatement(string statement, Action<CSharpStatement> configure = null)
    {
        var s = new CSharpStatement(statement);
        Statements.Add(s);
        configure?.Invoke(s);
        return this;
    }

    public CSharpConstructor InsertStatement(int index, string statement, Action<CSharpStatement> configure = null)
    {
        var s = new CSharpStatement(statement);
        Statements.Insert(index, s);
        configure?.Invoke(s);
        return this;
    }

    public CSharpConstructor AddStatements(string statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public CSharpConstructor AddStatements(IEnumerable<string> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new CSharpStatement(x)), configure);
    }

    public CSharpConstructor AddStatements(IEnumerable<CSharpStatement> statements, Action<IEnumerable<CSharpStatement>> configure = null)
    {
        var arrayed = statements.ToArray();
        foreach (var statement in arrayed)
        {
            Statements.Add(statement);
        }
        configure?.Invoke(arrayed);

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
        return $@"{indentation}{AccessModifier}{Class.Name}({ToStringParameters(indentation)}){ConstructorCall?.ToString() ?? string.Empty}
{indentation}{{{(Statements.Any() ? $@"
{indentation}    {string.Join($@"
", Statements.Select((s, index) => s.MustSeparateFromPrevious && index != 0 ? $@"
{indentation}   {s}" : $"{indentation}   {s}"))}" : string.Empty)}
{indentation}}}";
    }

    private string ToStringParameters(string indentation)
    {
        if (Parameters.Sum(x => x.ToString().Length) > 120)
        {
            return string.Join($@",
{indentation}    ", Parameters.Select(x => x.ToString()));
        }
        else
        {
            return string.Join(", ", Parameters.Select(x => x.ToString()));
        }
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
