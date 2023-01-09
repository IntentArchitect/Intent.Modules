using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptMethod : TypescriptMember<TypescriptMethod>, IHasTypescriptStatements
{
    public List<TypescriptStatement> Statements { get; } = new();
    protected string AsyncMode { get; private set; } = string.Empty;
    protected string AccessModifier { get; private set; } = string.Empty;
    protected string OverrideModifier { get; private set; } = string.Empty;
    public string ReturnType { get; }
    public string Name { get; }
    public List<TypescriptParameter> Parameters { get; } = new();
    public TypescriptMethod(string name, string returnType)
    {
        if (string.IsNullOrWhiteSpace(returnType))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(returnType));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        ReturnType = returnType;
        Name = name;
        BeforeSeparator = TypescriptCodeSeparatorType.EmptyLines;
        AfterSeparator = TypescriptCodeSeparatorType.EmptyLines;
    }

    public TypescriptMethod AddParameter(string name, string type, Action<TypescriptParameter> configure = null)
    {
        var param = new TypescriptParameter(name, type);
        Parameters.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public TypescriptMethod AddStatement(string statement, Action<TypescriptStatement> configure = null)
    {
        return AddStatement(new TypescriptStatement(statement), configure);
    }

    public TypescriptMethod AddStatement(TypescriptStatement statement, Action<TypescriptStatement> configure = null)
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public TypescriptMethod InsertStatement(int index, TypescriptStatement statement, Action<TypescriptStatement> configure = null)
    {
        Statements.Insert(index, statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public TypescriptMethod InsertStatements(int index, IReadOnlyCollection<TypescriptStatement> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        foreach (var s in statements.Reverse())
        {
            Statements.Insert(index, s);
            s.Parent = this;
        }
        configure?.Invoke(statements);
        return this;
    }

    public TypescriptMethod AddStatements(string statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public TypescriptMethod AddStatements(IEnumerable<string> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new TypescriptStatement(x)), configure);
    }

    public TypescriptMethod AddStatements(IEnumerable<TypescriptStatement> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        var arrayed = statements.ToArray();
        foreach (var statement in arrayed)
        {
            Statements.Add(statement);
            statement.Parent = this;
        }
        configure?.Invoke(arrayed);

        return this;
    }

    public TypescriptMethod FindAndReplaceStatement(Func<TypescriptStatement, bool> matchFunc, TypescriptStatement replaceWith)
    {
        this.FindStatement(matchFunc)?.Replace(replaceWith);
        return this;
    }

    public TypescriptMethod Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public TypescriptMethod Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public TypescriptMethod WithoutAccessModifier()
    {
        AccessModifier = "";
        return this;
    }

    public TypescriptMethod Override()
    {
        OverrideModifier = "override ";
        return this;
    }

    public TypescriptMethod Abstract()
    {
        OverrideModifier = "abstract ";
        return this;
    }

    public TypescriptMethod Static()
    {
        OverrideModifier = "static ";
        return this;
    }

    public TypescriptMethod Async()
    {
        AsyncMode = "async ";
        return this;
    }

    public void RemoveStatement(TypescriptStatement statement)
    {
        Statements.Remove(statement);
    }

    public override string GetText(string indentation)
    {
        return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{AccessModifier}{OverrideModifier}{AsyncMode}{Name}({string.Join(", ", Parameters.Select(x => x.ToString()))}): {ReturnType} {{{Statements.ConcatCode($"{indentation}    ")}
{indentation}}}";
    }
}