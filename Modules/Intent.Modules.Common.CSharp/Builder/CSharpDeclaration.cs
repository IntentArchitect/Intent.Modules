using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public abstract class CSharpDeclaration<TImpl> : CSharpMetadataBase<TImpl>
    where TImpl : CSharpDeclaration<TImpl>
{
    public IList<CSharpAttribute> Attributes { get; } = new List<CSharpAttribute>();
    public CSharpXmlComments XmlComments { get; } = new CSharpXmlComments();

    public TImpl AddAttribute(string name, Action<CSharpAttribute> configure = null)
    {
        return AddAttribute(new CSharpAttribute(name), configure);
    }

    public TImpl AddAttribute(CSharpAttribute attribute, Action<CSharpAttribute> configure = null)
    {
        Attributes.Add(attribute);
        configure?.Invoke(attribute);
        return (TImpl)this;
    }

    public TImpl WithComments(string xmlComments)
    {
        XmlComments.AddStatements(xmlComments);
        return (TImpl)this;
    }

    public TImpl WithComments(IEnumerable<string> xmlComments)
    {
        XmlComments.AddStatements(xmlComments);
        return (TImpl)this;
    }

    protected string GetAttributes(string indentation)
    {
        return $@"{(Attributes.Any() ? $@"{string.Join($@"
", Attributes.Select(x => x.GetText(indentation)))}
" : string.Empty)}";
    }

    protected string GetComments(string indentation)
    {
        return $@"{(!XmlComments.IsEmpty() ? $@"{XmlComments.ToString(indentation)}
" : string.Empty)}";
    }
}