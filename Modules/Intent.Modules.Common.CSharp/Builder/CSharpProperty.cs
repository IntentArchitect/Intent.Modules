using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpProperty : CSharpDeclaration<CSharpProperty>
{
    private readonly CSharpClass _class;
    public string AccessModifier { get; private set; } = "public ";
    public string OverrideModifier { get; private set; } = "";
    public string Type { get; }
    public string Name { get; }
    public bool IsReadOnly { get; private set; } = false;
    public string InitialValue { get; private set; }
    public CSharpPropertyAccessor Getter { get; } = CSharpPropertyAccessor.Getter();
    public CSharpPropertyAccessor Setter { get; } = CSharpPropertyAccessor.Setter();

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
            return $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{OverrideModifier}{Type} {Name}
{indentation}{{
{Getter.ToString(indentation + "    ")}{(!IsReadOnly ? $@"
{indentation}{Setter.ToString(indentation + "    ")}" : string.Empty)}
{indentation}}}{(InitialValue != null ? $" = {InitialValue};" : string.Empty)}";
        }
        return $@"{(!XmlComments.IsEmpty() ? $@"{XmlComments.ToString(indentation)}
" : string.Empty)}{indentation}{AccessModifier}{OverrideModifier}{Type} {Name} {{ {Getter}{(!IsReadOnly ? $" {Setter}" : string.Empty)} }}{(InitialValue != null ? $" = {InitialValue};" : string.Empty)}";
    }
}