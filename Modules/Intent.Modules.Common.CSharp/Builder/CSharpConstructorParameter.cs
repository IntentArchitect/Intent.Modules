using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpConstructorParameter : CSharpMetadataBase<CSharpConstructorParameter>, ICSharpConstructorParameter
{
    private readonly CSharpConstructor _constructor;
    public string Type { get; }
    public string Name { get; }
    public string? DefaultValue { get; private set; }
    public string XmlDocComment { get; private set; }
    public IList<CSharpAttribute> Attributes { get; } = new List<CSharpAttribute>();

    public CSharpConstructorParameter(string type, string name, CSharpConstructor constructor)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        _constructor = constructor;
        Parent = constructor;
        File = constructor.File;
        Type = type;
        Name = name;
    }

    public CSharpConstructorParameter WithXmlDocComment(IElement parameter)
    {
        return WithXmlDocComment(parameter?.Comment);
    }

    public CSharpConstructorParameter WithXmlDocComment(string comment)
    {
        if (!string.IsNullOrWhiteSpace(comment))
            XmlDocComment = comment;
        return this;
    }

    public CSharpConstructorParameter IntroduceField(Action<CSharpField> configure = null)
    {
        return IntroduceField((field, _) => configure?.Invoke(field));
    }

    public CSharpConstructorParameter IntroduceField(Action<CSharpField, CSharpFieldAssignmentStatement> configure)
    {
        if (_constructor.IsPrimaryConstructor)
        {
            throw new InvalidOperationException($"Introducing a backing field for a primary constructor parameter is not allowed. {_constructor.Class.TypeDefinitionType}: {_constructor.Class.Name}.");
        }
        
        _constructor.Class.AddField(Type, Name.ToPrivateMemberName(), field =>
        {
            foreach (var kvp in Metadata)
            {
                field.AddMetadata(kvp.Key, kvp.Value);
            }
            var statement = new CSharpFieldAssignmentStatement(field.Name, Name);
            _constructor.AddStatement(statement);
            configure?.Invoke(field, statement);
        });
        return this;
    }

    public CSharpConstructorParameter IntroduceReadonlyField(Action<CSharpField> configure = null)
    {
        return IntroduceReadonlyField((field, _) => configure?.Invoke(field));
    }

    public CSharpConstructorParameter IntroduceReadonlyField(Action<CSharpField, CSharpFieldAssignmentStatement> configure)
    {
        if (_constructor.IsPrimaryConstructor)
        {
            throw new InvalidOperationException($"Introducing a backing field for a primary constructor parameter is not allowed. {_constructor.Class.TypeDefinitionType}: {_constructor.Class.Name}.");
        }
        
        return IntroduceField((field, statement) =>
        {
            field.PrivateReadOnly();
            configure?.Invoke(field, statement);
        });
    }

    public CSharpConstructorParameter IntroduceProperty(Action<CSharpProperty> configure = null)
    {
        return IntroduceProperty((property, _) => configure?.Invoke(property));
    }

    public CSharpConstructorParameter IntroduceProperty(Action<CSharpProperty, CSharpFieldAssignmentStatement> configure)
    {
        if (_constructor.IsPrimaryConstructor)
        {
            throw new InvalidOperationException($"Introducing a property for a primary constructor parameter is not allowed. {_constructor.Class.TypeDefinitionType}: {_constructor.Class.Name}.");
        }
        
        _constructor.Class.AddProperty(Type, Name.ToPropertyName(), property =>
        {
            foreach (var kvp in Metadata)
            {
                property.AddMetadata(kvp.Key, kvp.Value);
            }
            var statement = new CSharpFieldAssignmentStatement(property.Name, Name);
            _constructor.AddStatement(statement);
            configure?.Invoke(property, statement);
        });
        return this;
    }

    public CSharpConstructorParameter WithDefaultValue(string defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    public CSharpConstructorParameter AddAttribute(string name, Action<CSharpAttribute> configure = null)
    {
        var param = new CSharpAttribute(name);
        Attributes.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public override string ToString()
    {
        var name = Name.EnsureNotKeyword();
        var defaultValue = DefaultValue != null
            ? $" = {DefaultValue}"
            : string.Empty;

        return $@"{GetAttributes()}{Type} {name}{defaultValue}";
    }
    
    protected string GetAttributes()
    {
        return $@"{(Attributes.Any() ? $@"{string.Join(@" ", Attributes)} " : string.Empty)}";
    }

    #region ICSharpConstructorParameter implementation

    ICSharpConstructorParameter ICSharpConstructorParameter.IntroduceField(Action<ICSharpField> configure) => IntroduceField(configure);

    ICSharpConstructorParameter ICSharpConstructorParameter.IntroduceField(Action<ICSharpField, ICSharpFieldAssignmentStatement> configure) => IntroduceField(configure);

    ICSharpConstructorParameter ICSharpConstructorParameter.IntroduceReadonlyField(Action<ICSharpField> configure) => IntroduceReadonlyField(configure);

    ICSharpConstructorParameter ICSharpConstructorParameter.IntroduceReadonlyField(Action<ICSharpField, ICSharpFieldAssignmentStatement> configure) => IntroduceReadonlyField(configure);

    ICSharpConstructorParameter ICSharpConstructorParameter.IntroduceProperty(Action<ICSharpProperty> configure) => IntroduceProperty(configure);

    ICSharpConstructorParameter ICSharpConstructorParameter.IntroduceProperty(Action<ICSharpProperty, ICSharpFieldAssignmentStatement> configure) => IntroduceProperty(configure);

    #endregion
}