#nullable enable
using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpMethodParameter : ICSharpParameter
{
    public ICSharpMethodParameter AddAttribute(string name, Action<ICSharpAttribute>? configure = null);
    public ICSharpMethodParameter WithXmlDocComment(IElement parameter);
    public ICSharpMethodParameter WithXmlDocComment(string comment);
    public ICSharpMethodParameter WithDefaultValue(string defaultValue);
    public ICSharpMethodParameter WithThisModifier();
    public ICSharpMethodParameter WithOutParameterModifier();
    public ICSharpMethodParameter WithInParameterModifier();
    public ICSharpMethodParameter WithRefParameterModifier();
    public ICSharpMethodParameter WithParamsParameterModifier();
    public IList<ICSharpAttribute> Attributes { get; }
    public bool HasThisModifier { get; }
    public string ParameterModifier { get; }
    public string GetReferenceName();
}