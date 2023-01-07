using System;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpProperty : CSharpMember<CSharpProperty>
{
    private readonly CSharpClass _class;
    public string AccessModifier { get; private set; } = "public ";
    public string OverrideModifier { get; private set; } = "";
    public string Type { get; }
    public string Name { get; }
    public bool IsReadOnly { get; private set; }
    public string InitialValue { get; private set; }
    public CSharpPropertyAccessor Getter { get; } = CSharpPropertyAccessor.Getter();
    public CSharpPropertyAccessor Setter { get; } = CSharpPropertyAccessor.Setter();

    public CSharpProperty(string type, string name, CSharpClass @class)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Type = type;
        Name = name;
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        AfterSeparator = CSharpCodeSeparatorType.NewLine;
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

    public CSharpProperty WithoutAccessModifier()
    {
        AccessModifier = "";
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

    public CSharpProperty WithoutSetter()
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

    public CSharpProperty MoveTo(int propertyIndex)
    {
        _class.Properties.Remove(this);
        _class.Properties.Insert(propertyIndex, this);
        return this;
    }

    public CSharpProperty MoveToFirst()
    {
        return MoveTo(0);
    }

    public CSharpProperty MoveToLast()
    {
        return MoveTo(_class.Properties.Count - 1);
    }

    public override string GetText(string indentation)
    {
        var declaration = $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{AccessModifier}{OverrideModifier}{Type} {Name}";
        if (Getter.IsExpression && IsReadOnly)
        {
            return $@"{declaration} => {Getter.Implementation};";
        }

        if (!Getter.Implementation.IsEmpty() || !Setter.Implementation.IsEmpty())
        {
            return $@"{declaration}
{indentation}{{
{Getter.ToString(indentation + "    ")}{(!IsReadOnly ? $@"
{indentation}{Setter.ToString(indentation + "    ")}" : string.Empty)}
{indentation}}}{(InitialValue != null ? $" = {InitialValue};" : string.Empty)}";
        }
        return $@"{declaration} {{ {Getter}{(!IsReadOnly ? $" {Setter}" : string.Empty)} }}{(InitialValue != null ? $" = {InitialValue};" : string.Empty)}";
    }
}