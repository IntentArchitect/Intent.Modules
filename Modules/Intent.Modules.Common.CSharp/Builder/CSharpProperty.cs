using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Xml;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpProperty
{
    public string AccessModifier { get; private set; } = "public ";
    public string OverrideModifier { get; private set; } = "";
    public string Type { get; }
    public string Name { get; }
    public bool IsReadOnly { get; private set; } = false;
    public CSharpStatements XmlComments { get; private set; } = new CSharpStatements();
    public CSharpClass Class { get; }
    public CSharpPropertyAccessor Getter { get; private set; } = CSharpPropertyAccessor.Getter();
    public CSharpPropertyAccessor Setter { get; private set; } = CSharpPropertyAccessor.Setter();

    public CSharpProperty(string type, string name, CSharpClass @class)
    {
        Type = type;
        Name = name;
        Class = @class;
    }

    public CSharpProperty Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public CSharpProperty Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public CSharpProperty Override()
    {
        OverrideModifier = "override ";
        return this;
    }

    public CSharpProperty New()
    {
        OverrideModifier = "new ";
        return this;
    }

    public CSharpProperty Virtual()
    {
        OverrideModifier = "virtual ";
        return this;
    }

    public CSharpProperty PrivateSetter()
    {
        Setter.Private();
        return this;
    }

    public CSharpProperty WithComments(string xmlComments)
    {
        XmlComments.AddStatements(xmlComments.Split('\n', '\r', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray());
        return this;
    }

    public CSharpProperty WithComments(params string[] xmlComments)
    {
        XmlComments.AddStatements(xmlComments);
        return this;
    }

    public CSharpProperty WithBackingField(Action<CSharpField> configure = null)
    {
        Getter.WithExpressionImplementation(Name.ToPrivateMemberName());
        Setter.WithExpressionImplementation($"{Name.ToPrivateMemberName()} = value");
        var field = Class.AddField(Type, Name.ToPrivateMemberName()).Private();
        configure?.Invoke(field);
        return this;
    }

    public string ToString(string indentation)
    {
        if (Getter.IsExpression && IsReadOnly)
        {
            return $@"{indentation}{AccessModifier}{OverrideModifier}{Type} {Name} => {Getter};";
        }

        if (!Getter.Implementation.IsEmpty() || !Setter.Implementation.IsEmpty())
        {
            return $@"{indentation}{(!XmlComments.IsEmpty() ? XmlComments.ToString(indentation) + $@"
{indentation}" : string.Empty)}{AccessModifier}{OverrideModifier}{Type} {Name} 
{indentation}{{ 
{Getter.ToString(indentation + "    ")}{(!IsReadOnly ? $@"
{indentation}{Setter.ToString(indentation + "    ")}" : string.Empty)}
{indentation}}}";
        }
        return $@"{indentation}{(!XmlComments.IsEmpty() ? XmlComments.ToString(indentation) + $@"
{indentation}" : string.Empty)}{AccessModifier}{OverrideModifier}{Type} {Name} {{ {Getter}{(!IsReadOnly ? Setter : string.Empty)} }}";
    }
}

public class CSharpPropertyAccessor
{
    public string Accessor { get; }
    public string AccessModifier { get; private set; } = "";
    public bool IsExpression { get; private set; } = false;

    public CSharpStatements Implementation { get; private set; } = new CSharpStatements();

    private CSharpPropertyAccessor(string accessor)
    {
        Accessor = accessor;
    }

    public static CSharpPropertyAccessor Getter()
    {
        return new CSharpPropertyAccessor("get");
    }

    public static CSharpPropertyAccessor Setter()
    {
        return new CSharpPropertyAccessor("set");
    }

    public CSharpPropertyAccessor Public()
    {
        AccessModifier = "public ";
        return this;
    }
    public CSharpPropertyAccessor Protected()
    {
        AccessModifier = "protected ";
        return this;
    }
    public CSharpPropertyAccessor Private()
    {
        AccessModifier = "private ";
        return this;
    }

    public CSharpPropertyAccessor WithImplementation(params string[] statements)
    {
        Implementation = new CSharpStatements(statements);
        IsExpression = statements.Length == 1 && !statements[0].Contains("return");
        return this;
    }

    public CSharpPropertyAccessor WithBodyImplementation(params string[] statements)
    {
        Implementation = new CSharpStatements(statements);
        IsExpression = false;
        return this;
    }

    public CSharpPropertyAccessor WithExpressionImplementation(string implementation)
    {
        Implementation = new CSharpStatements(new[] { implementation });
        IsExpression = true;
        return this;
    }

    public override string ToString()
    {
        return ToString(string.Empty);
    }

    public string ToString(string indentation)
    {
        if (Implementation.IsEmpty())
        {
            return $@"{AccessModifier}{Accessor};";
        }

        if (IsExpression)
        {
            return $"{indentation}{Accessor} => {Implementation};";
        }
        else if (Implementation.Statements.Count == 1)
        {
            return $"{indentation}{Accessor} {{ {Implementation} }}";
        }
        else
        {
            return @$"{indentation}{Accessor}
{indentation}{{ 
{Implementation.ToString($"{indentation}    ")} 
{indentation}}}";
        }
    }
}

public class CSharpStatements
{
    public readonly IList<string> Statements = new List<string>();

    public CSharpStatements(params string[] statements)
    {
        ((List<string>)Statements).AddRange(statements);
    }

    public CSharpStatements AddStatements(params string[] statements)
    {
        ((List<string>)Statements).AddRange(statements);
        return this;
    }

    public bool IsEmpty() => !Statements.Any();

    public override string ToString()
    {
        return ToString(string.Empty);
    }

    public string ToString(string indentation)
    {
        return $@"{(Statements.Any() ? $@"{indentation}{string.Join($@"
{indentation}", Statements)}" : string.Empty)}";
    }
}