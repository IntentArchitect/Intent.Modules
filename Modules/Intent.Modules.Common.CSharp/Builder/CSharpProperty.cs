#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpProperty : CSharpMember<CSharpProperty>, ICSharpProperty
{
    private readonly CSharpClass _class;
    private readonly ICSharpProperty _wrapper;
    public string AccessModifier { get; protected set; } = "public ";
    public string OverrideModifier { get; private set; } = "";
    public string Type { get; }
    public string Name { get; }
    public bool IsReadOnly { get; private set; }
    public bool IsStatic { get; private set; }
    public bool IsRequired { get; private set; }
    public string InitialValue { get; private set; }
    public string ExplicitlyImplementing { get; private set; }
    public bool IsOmittedFromRender { get; private set; }
    public CSharpPropertyAccessor Getter { get; } = CSharpPropertyAccessor.Getter();
    public CSharpPropertyAccessor Setter { get; } = CSharpPropertyAccessor.Setter();

    public CSharpProperty(string type, string name, ICSharpCodeContext @class)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        _wrapper = new CSharpPropertyWrapper(this);
        Type = type;
        Name = name;
        BeforeSeparator = CSharpCodeSeparatorType.NewLine;
        AfterSeparator = CSharpCodeSeparatorType.NewLine;
        _class = @class as CSharpClass;
        Parent = @class;
        File = @class?.File; // can be null because of CSharpInterfaceProperty :(
    }

    internal static CSharpProperty CreatePropertyOmittedFromRender(string type, string name, CSharpClass @class, bool hasInitSetter = true)
    {
        var property = new CSharpProperty(type,name,@class);
        property.IsOmittedFromRender = true;
        if (hasInitSetter)
        {
            property.Init();
        }
        return property;
    }

    public string GetReferenceName()
    {
        return Name;
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
    
    public CSharpProperty Static()
    {
        IsStatic = true;
        return this;
    }
    public CSharpProperty Required()
    {
        IsRequired = true;
        return this;
    }

    public CSharpProperty PrivateSetter()
    {
        Setter.Private();
        return this;
    }

    public CSharpProperty ProtectedSetter()
    {
        Setter.Protected();
        return this;
    }

    public CSharpProperty Init()
    {
        Setter.Init();
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

    public CSharpProperty ExplicitlyImplements(string @interface)
    {
        ExplicitlyImplementing = @interface;
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

    public CSharpProperty WithInstantiation()
    {
        var propertyType = CSharpTypeParser.Parse(Type);

        if (File?.Template.OutputTarget.GetProject().GetLanguageVersion().Major >= 12 && propertyType is not null && propertyType.IsCollectionType())
        {
            InitialValue = "[]";
            return this;
        }

        if (File?.Template.OutputTarget.GetProject().GetLanguageVersion().Major < 12 && propertyType is not null && propertyType.IsCollectionType())
        {
            var concreteImplementation = propertyType.GetCollectionImplementationType();
            InitialValue = GetInstantiationValue(propertyType, concreteImplementation);

            return this;
        }

        return this;
    }

    private string GetInstantiationValue(CSharpType? propertyType, ICSharpType concreteImplementation)
    {
        if(propertyType is CSharpTypeGeneric generic)
        {
            var argTypes = string.Join(", ", generic.TypeArgumentList.Select(s => File.Template.UseType(s.ToString())));
            return $"new {File.Template.UseType($"{concreteImplementation}<{argTypes}>").Replace("?", "")}()";
        }
        
        return $"new {File.Template.UseType($"{concreteImplementation}").Replace("?", "")}()";
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
        var modifiers = !string.IsNullOrWhiteSpace(ExplicitlyImplementing)
            ? string.Empty
            : $"{AccessModifier}{OverrideModifier}";

        var explicitlyImplementing = !string.IsNullOrWhiteSpace(ExplicitlyImplementing)
            ? $"{ExplicitlyImplementing}."
            : string.Empty;

        var declaration = $@"{GetComments(indentation)}{GetAttributes(indentation)}{indentation}{modifiers}{(IsStatic ? "static " : "")}{(IsRequired ? "required " : "")}{Type} {explicitlyImplementing}{Name}";
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

    #region ICSharpProperty implementation

    ICSharpPropertyAccessor ICSharpProperty.Getter => _wrapper.Getter;

    ICSharpPropertyAccessor ICSharpProperty.Setter => _wrapper.Setter;

    IEnumerable<ICSharpAttribute> ICSharpDeclaration<ICSharpProperty>.Attributes => Attributes;

    ICSharpProperty ICSharpDeclaration<ICSharpProperty>.AddAttribute(string name, Action<ICSharpAttribute> configure) => _wrapper.AddAttribute(name, configure);

    ICSharpProperty ICSharpDeclaration<ICSharpProperty>.AddAttribute(ICSharpAttribute attribute, Action<ICSharpAttribute> configure) => _wrapper.AddAttribute(attribute, configure);

    ICSharpProperty ICSharpDeclaration<ICSharpProperty>.WithComments(string xmlComments) => _wrapper.WithComments(xmlComments);

    ICSharpProperty ICSharpDeclaration<ICSharpProperty>.WithComments(IEnumerable<string> xmlComments) => _wrapper.WithComments(xmlComments);

    ICSharpProperty ICSharpProperty.Protected() => _wrapper.Protected();

    ICSharpProperty ICSharpProperty.Private() => _wrapper.Private();

    ICSharpProperty ICSharpProperty.WithoutAccessModifier() => _wrapper.WithoutAccessModifier();

    ICSharpProperty ICSharpProperty.Override() => _wrapper.Override();

    ICSharpProperty ICSharpProperty.New() => _wrapper.New();

    ICSharpProperty ICSharpProperty.Virtual() => _wrapper.Virtual();

    ICSharpProperty ICSharpProperty.Static() => _wrapper.Static();

    ICSharpProperty ICSharpProperty.Required() => _wrapper.Required();

    ICSharpProperty ICSharpProperty.PrivateSetter() => _wrapper.PrivateSetter();

    ICSharpProperty ICSharpProperty.ProtectedSetter() => _wrapper.ProtectedSetter();

    ICSharpProperty ICSharpProperty.Init() => _wrapper.Init();

    ICSharpProperty ICSharpProperty.ReadOnly() => _wrapper.ReadOnly();

    ICSharpProperty ICSharpProperty.WithoutSetter() => _wrapper.WithoutSetter();

    ICSharpProperty ICSharpProperty.WithInitialValue(string initialValue) => _wrapper.WithInitialValue(initialValue);

    ICSharpProperty ICSharpProperty.ExplicitlyImplements(string @interface) => _wrapper.ExplicitlyImplements(@interface);

    ICSharpProperty ICSharpProperty.WithBackingField(Action<ICSharpField>? configure) => _wrapper.WithBackingField(configure);

    ICSharpProperty ICSharpProperty.MoveTo(int propertyIndex) => _wrapper.MoveTo(propertyIndex);

    ICSharpProperty ICSharpProperty.MoveToFirst() => _wrapper.MoveToFirst();

    ICSharpProperty ICSharpProperty.MoveToLast() => _wrapper.MoveToLast();

    ICSharpProperty ICSharpProperty.WithInstantiation() => _wrapper.WithInstantiation();

    #endregion
}