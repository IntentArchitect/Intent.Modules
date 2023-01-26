using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Builder;

public class JavaConstructor : JavaMember<JavaConstructor>
{
    public JavaClass Class { get; }
    public string AccessModifier { get; private set; } = "public ";
    public JavaConstructorCall ConstructorCall { get; private set; }
    public IList<JavaConstructorParameter> Parameters { get; } = new List<JavaConstructorParameter>();
    public List<JavaStatement> Statements { get; } = new();
    public JavaConstructor(JavaClass @class)
    {
        BeforeSeparator = JavaCodeSeparatorType.EmptyLines;
        AfterSeparator = JavaCodeSeparatorType.EmptyLines;
        Class = @class;
    }

    public JavaConstructor AddParameter(string type, string name, Action<JavaConstructorParameter> configure = null)
    {
        var param = new JavaConstructorParameter(type, name, this);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public JavaConstructor AddStatement(JavaStatement statement, Action<JavaStatement> configure = null)
    {
        Statements.Add(statement);
        configure?.Invoke(statement);
        return this;
    }

    public JavaConstructor InsertStatement(int index, string statement, Action<JavaStatement> configure = null)
    {
        var s = new JavaStatement(statement);
        Statements.Insert(index, s);
        configure?.Invoke(s);
        return this;
    }

    public JavaConstructor AddStatements(string statements, Action<IEnumerable<JavaStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public JavaConstructor AddStatements(IEnumerable<string> statements, Action<IEnumerable<JavaStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new JavaStatement(x)), configure);
    }

    public JavaConstructor AddStatements(IEnumerable<JavaStatement> statements, Action<IEnumerable<JavaStatement>> configure = null)
    {
        var arrayed = statements.ToArray();
        foreach (var statement in arrayed)
        {
            Statements.Add(statement);
        }
        configure?.Invoke(arrayed);

        return this;
    }

    public JavaConstructor Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public JavaConstructor Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public JavaConstructor CallsSuper(Action<JavaConstructorCall> configure = null)
    {
        ConstructorCall = JavaConstructorCall.Super();
        configure?.Invoke(ConstructorCall);
        return this;
    }

    public JavaConstructor CallsThis(Action<JavaConstructorCall> configure = null)
    {
        ConstructorCall = JavaConstructorCall.This();
        configure?.Invoke(ConstructorCall);
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetAnnotations(indentation)}{indentation}{AccessModifier}{Class.Name}({ToStringParameters(indentation)}) {{{ConstructorCall?.ToString() ?? string.Empty}{Statements.ConcatCode($"{indentation}    ")}
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