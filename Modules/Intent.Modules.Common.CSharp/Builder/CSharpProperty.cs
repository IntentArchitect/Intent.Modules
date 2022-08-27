using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpProperty
{
    private readonly CSharpClass _class;
    public string AccessModifier { get; private set; } = "public ";
    public string OverrideModifier { get; private set; } = "";
    public string Type { get; }
    public string Name { get; }
    public bool IsReadOnly { get; private set; } = false;
    public string InitialValue { get; private set; }
    public CSharpXmlComments XmlComments { get; } = new CSharpXmlComments();
    public CSharpPropertyAccessor Getter { get; } = CSharpPropertyAccessor.Getter();
    public CSharpPropertyAccessor Setter { get; } = CSharpPropertyAccessor.Setter();
    public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

    public CSharpProperty(string type, string name, CSharpClass @class)
    {
        Type = type;
        Name = name;
        _class = @class;
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

    public CSharpProperty ReadOnly()
    {
        IsReadOnly = true;
        return this;
    }

    public CSharpProperty WithInitialValue(string initialValue)
    {
        InitialValue = initialValue;
        return this;
    }

    public CSharpProperty WithComments(string xmlComments)
    {
        XmlComments.AddStatements(xmlComments);
        return this;
    }

    public CSharpProperty WithComments(IEnumerable<string> xmlComments)
    {
        XmlComments.AddStatements(xmlComments);
        return this;
    }

    public CSharpProperty AddMetadata<T>(string key, T value)
    {
        Metadata.Add(key, value);
        return this;
    }

    public bool TryGetMetadata<T>(string key, out T value)
    {
        if (Metadata.TryGetValue(key, out var valueFound) && valueFound is T castValue)
        {
            value = castValue;
            return true;
        }

        value = default(T);
        return false;
    }

    public bool TryGetMetadata(string key, out object value)
    {
        if (Metadata.TryGetValue(key, out var valueFound))
        {
            value = valueFound;
            return true;
        }

        value = null;
        return false;
    }

    public CSharpProperty WithBackingField(Action<CSharpField> configure = null)
    {
        Getter.WithExpressionImplementation(Name.ToPrivateMemberName());
        Setter.WithExpressionImplementation($"{Name.ToPrivateMemberName()} = value");
        _class.AddField(Type, Name.ToPrivateMemberName(), field =>
        {
            configure?.Invoke(field);
        });
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
            return $@"{(!XmlComments.IsEmpty() ? $@"{XmlComments.ToString(indentation)}
" : string.Empty)}{indentation}{AccessModifier}{OverrideModifier}{Type} {Name}
{indentation}{{
{Getter.ToString(indentation + "    ")}{(!IsReadOnly ? $@"
{indentation}{Setter.ToString(indentation + "    ")}" : string.Empty)}
{indentation}}}{(InitialValue != null ? $" = {InitialValue};" : string.Empty)}";
        }
        return $@"{(!XmlComments.IsEmpty() ? $@"{XmlComments.ToString(indentation)}
" : string.Empty)}{indentation}{AccessModifier}{OverrideModifier}{Type} {Name} {{ {Getter}{(!IsReadOnly ? $" {Setter}" : string.Empty)} }}{(InitialValue != null ? $" = {InitialValue};" : string.Empty)}";
    }
}