using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpConstructor : CSharpMember<CSharpConstructor>, IHasCSharpStatements, IHasICSharpParameters
{
    public CSharpClass Class { get; }
    public string AccessModifier { get; private set; } = "public ";
    public CSharpConstructorCall ConstructorCall { get; private set; }
    public IList<CSharpConstructorParameter> Parameters { get; } = new List<CSharpConstructorParameter>();
    public List<CSharpStatement> Statements { get; } = new();

    IList<CSharpStatement> IHasCSharpStatements.Statements => this.Statements;
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => this.Parameters;

    public CSharpConstructor(CSharpClass @class)
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
        Class = @class;
    }

    public CSharpConstructor AddParameter(string type, string name, Action<CSharpConstructorParameter> configure = null)
    {
        var param = new CSharpConstructorParameter(type, name, this);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpConstructor AddStatement(CSharpStatement statement, Action<CSharpStatement> configure = null)
    {
        Statements.Add(statement);
        configure?.Invoke(statement);
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

    public CSharpConstructor Static()
    {
        AccessModifier = "static ";
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

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{Class.Name}({ToStringParameters(indentation)}){ConstructorCall?.ToString() ?? string.Empty}
{indentation}{{{Statements.ConcatCode($"{indentation}    ")}
{indentation}}}";
    }

    private string ToStringParameters(string indentation)
    {
        if (Parameters.Count > 1 && $"{indentation}{AccessModifier}{Class.Name}(".Length + Parameters.Sum(x => x.ToString().Length) > 120)
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