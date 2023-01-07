using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public class TypescriptAccessor : TypescriptMember<TypescriptAccessor>, IHasTypescriptStatements
{
    public string Accessor { get; }
    public string AccessModifier { get; private set; } = string.Empty;
    public string OverrideModifier { get; private set; } = string.Empty;
    public string Type { get; }
    public string Name { get; }

    public List<TypescriptStatement> Statements { get; } = new();

    private TypescriptAccessor(string accessor, string type, string name)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Accessor = accessor;
        Type = type;
        Name = name;
        BeforeSeparator = TypescriptCodeSeparatorType.NewLine;
        AfterSeparator = TypescriptCodeSeparatorType.NewLine;
    }

    public static TypescriptAccessor Getter(string type, string name)
    {
        return new TypescriptAccessor("get", type, name);
    }

    public static TypescriptAccessor Setter(string type, string name)
    {
        return new TypescriptAccessor("set", type, name);
    }

    public TypescriptAccessor Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public TypescriptAccessor Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public TypescriptAccessor WithoutAccessModifier()
    {
        AccessModifier = string.Empty;
        return this;
    }

    public TypescriptAccessor Override()
    {
        OverrideModifier = "override ";
        return this;
    }

    public TypescriptAccessor AddStatement(string statement, Action<TypescriptStatement> configure = null)
    {
        return AddStatement(new TypescriptStatement(statement), configure);
    }

    public TypescriptAccessor AddStatement(TypescriptStatement statement, Action<TypescriptStatement> configure = null)
    {
        Statements.Add(statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public TypescriptAccessor InsertStatement(int index, TypescriptStatement statement, Action<TypescriptStatement> configure = null)
    {
        Statements.Insert(index, statement);
        statement.Parent = this;
        configure?.Invoke(statement);
        return this;
    }

    public TypescriptAccessor InsertStatements(int index, IReadOnlyCollection<TypescriptStatement> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        foreach (var s in statements.Reverse())
        {
            Statements.Insert(index, s);
            s.Parent = this;
        }
        configure?.Invoke(statements);
        return this;
    }

    public TypescriptAccessor AddStatements(string statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        return AddStatements(statements.ConvertToStatements(), configure);
    }

    public TypescriptAccessor AddStatements(IEnumerable<string> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
    {
        return AddStatements(statements.Select(x => new TypescriptStatement(x)), configure);
    }

    public TypescriptAccessor AddStatements(IEnumerable<TypescriptStatement> statements, Action<IEnumerable<TypescriptStatement>> configure = null)
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

    public TypescriptAccessor FindAndReplaceStatement(Func<TypescriptStatement, bool> matchFunc, TypescriptStatement replaceWith)
    {
        this.FindStatement(matchFunc)?.Replace(replaceWith);
        return this;
    }

    public override string GetText(string indentation)
    {
        var (returnType, parameter) = Accessor switch
        {
            "get" => (string.Empty, $": {Type}"),
            "set" => ($"value: {Type}", string.Empty),
            _ => throw new ArgumentOutOfRangeException(nameof(Accessor), Accessor, "Out of range")
        };

        return $@"{GetComments(indentation)}{GetDecorators(indentation)}{indentation}{AccessModifier}{OverrideModifier}{Accessor} {Name}({parameter}){returnType} {{
{indentation}{Statements.ConcatCode($"{indentation}    ")}
{indentation}}}";
    }
}