using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpParameter : CSharpMetadataBase<CSharpParameter>, ICSharpMethodParameter
{
    public string Type { get; private set; }
    public string Name { get; }
    public string DefaultValue { get; private set; }
    public IList<CSharpAttribute> Attributes { get; } = new List<CSharpAttribute>();

    public bool HasThisModifier { get; private set; }
    public string XmlDocComment { get; private set; }
    public string ParameterModifier { get; private set; } = "";

    public CSharpParameter(string type, string name, CSharpMetadataBase parent)
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
        File = parent.File;
        Parent = parent;
    }

    public CSharpParameter(string type, string name, CSharpInterfaceMethod method)
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
        File = method?.File;
    }


    public string GetReferenceName()
    {
        return Name;
    }

    public CSharpParameter AddAttribute(string name, Action<CSharpAttribute> configure = null)
    {
        var param = new CSharpAttribute(name);
        Attributes.Add(param);
        configure?.Invoke(param);
        return this;
    }

    public CSharpParameter WithXmlDocComment(IElement parameter)
    {
        return WithXmlDocComment(parameter?.Comment);
    }

    public CSharpParameter WithXmlDocComment(string comment)
    {
        if (!string.IsNullOrWhiteSpace(comment))
            XmlDocComment = comment;
        return this;
    }

    public CSharpParameter WithDefaultValue(string defaultValue)
    {
        DefaultValue = string.IsNullOrWhiteSpace(defaultValue) ? null : defaultValue;
        return this;
    }

    public CSharpParameter WithThisModifier()
    {
        HasThisModifier = true;
        return this;
    }

    public CSharpParameter WithOutParameterModifier()
    {
        ParameterModifier = "out ";
        return this;
    }

    public CSharpParameter WithInParameterModifier()
    {
        ParameterModifier = "";
        return this;
    }

    public CSharpParameter WithRefParameterModifier()
    {
        ParameterModifier = "ref ";
        return this;
    }

    public CSharpParameter WithParamsParameterModifier()
    {
        ParameterModifier = "params ";
        return this;
    }

    public CSharpParameter WithType(string type)
    {
        Type = type;
        return this;
    }

    public override string ToString()
    {
        var name = Name.EnsureNotKeyword();
        var modifier = HasThisModifier
            ? "this "
            : string.Empty;
        var defaultValue = DefaultValue != null
            ? $" = {DefaultValue}"
            : string.Empty;

        return $@"{modifier}{GetAttributes()}{ParameterModifier}{Type} {name}{defaultValue}";
    }

    protected string GetAttributes()
    {
        return $@"{(Attributes.Any() ? $@"{string.Join(@" ", Attributes)} " : string.Empty)}";
    }

    #region ICSharpParameter implementation

    IList<ICSharpAttribute> ICSharpMethodParameter.Attributes => new WrappedList<CSharpAttribute,ICSharpAttribute>(Attributes);

    ICSharpMethodParameter ICSharpMethodParameter.AddAttribute(string name, Action<ICSharpAttribute> configure) => AddAttribute(name, configure);

    ICSharpMethodParameter ICSharpMethodParameter.WithXmlDocComment(IElement parameter) => WithXmlDocComment(parameter);

    ICSharpMethodParameter ICSharpMethodParameter.WithXmlDocComment(string comment) => WithXmlDocComment(comment);

    ICSharpMethodParameter ICSharpMethodParameter.WithDefaultValue(string defaultValue) => WithDefaultValue(defaultValue);

    ICSharpMethodParameter ICSharpMethodParameter.WithThisModifier() => WithThisModifier();

    ICSharpMethodParameter ICSharpMethodParameter.WithOutParameterModifier() => WithOutParameterModifier();

    ICSharpMethodParameter ICSharpMethodParameter.WithInParameterModifier() => WithInParameterModifier();

    ICSharpMethodParameter ICSharpMethodParameter.WithRefParameterModifier() => WithRefParameterModifier();

    ICSharpMethodParameter ICSharpMethodParameter.WithParamsParameterModifier() => WithParamsParameterModifier();

    #endregion
}