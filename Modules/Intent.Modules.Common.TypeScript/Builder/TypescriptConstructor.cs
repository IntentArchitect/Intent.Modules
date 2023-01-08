using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptConstructor : TypescriptMember<TypescriptConstructor>
{
    public TypescriptClass Class { get; }
    public string AccessModifier { get; private set; } = string.Empty;
    public TypescriptSuperConstructorCall SuperConstructorCall { get; private set; }
    public IList<TypescriptConstructorParameter> Parameters { get; } = new List<TypescriptConstructorParameter>();
    public List<TypescriptStatement> Statements { get; } = new();
    public TypescriptConstructor(TypescriptClass @class)
    {
        BeforeSeparator = TypescriptCodeSeparatorType.EmptyLines;
        AfterSeparator = TypescriptCodeSeparatorType.EmptyLines;
        Class = @class;
    }

    public TypescriptConstructor AddParameter(string type, string name, Action<TypescriptConstructorParameter> configure = null)
    {
        var param = new TypescriptConstructorParameter(type, name);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public TypescriptConstructor AddStatement(TypescriptStatement statement, Action<TypescriptStatement> configure = null)
    {
        Statements.Add(statement);
        configure?.Invoke(statement);
        return this;
    }

    public TypescriptConstructor InsertStatement(int index, string statement, Action<TypescriptStatement> configure = null)
    {
        var s = new TypescriptStatement(statement);
        Statements.Insert(index, s);
        configure?.Invoke(s);
        return this;
    }

    public TypescriptConstructor AddStatements(string statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public TypescriptConstructor AddStatements(IEnumerable<string> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new TypescriptStatement(x)), configure);
    }

    public TypescriptConstructor AddStatements(IEnumerable<TypescriptStatement> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        var arrayed = statements.ToArray();
        foreach (var statement in arrayed)
        {
            Statements.Add(statement);
        }
        configure?.Invoke(arrayed);

        return this;
    }

    public TypescriptConstructor Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public TypescriptConstructor Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public TypescriptConstructor CallsSuper(Action<TypescriptSuperConstructorCall> configure = null)
    {
        SuperConstructorCall = new TypescriptSuperConstructorCall();
        configure?.Invoke(SuperConstructorCall);
        return this;
    }

    public override string GetText(string indentation)
    {
        var statements = Statements as IReadOnlyCollection<ICodeBlock>;
        if (SuperConstructorCall != null)
        {
            statements = Enumerable.Empty<ICodeBlock>()
                .Append(SuperConstructorCall)
                .Concat(statements)
                .ToArray();
        }

        return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{AccessModifier}constructor({ToStringParameters(indentation)}) {{
{indentation}{statements.ConcatCode($"{indentation}    ")}
{indentation}}}";
    }

    private string ToStringParameters(string indentation)
    {
        var separator = Parameters.Sum(x => x.ToString().Length) > 120
            ? $",{Environment.NewLine}{indentation}    "
            : ", ";

        return string.Join(separator, Parameters.Select(x => x.ToString()));
    }
}