using Intent.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public abstract class CSharpDeclaration<TImpl> : CSharpMetadataBase<TImpl>
    where TImpl : CSharpDeclaration<TImpl>
{
    public IList<CSharpAttribute> Attributes { get; } = new List<CSharpAttribute>();
    public CSharpXmlComments XmlComments { get; } = new();

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
        return $@"{(Attributes.Any() ? $@"{string.Join(@"
", Attributes.Select(x => x.GetText(indentation)))}
" : string.Empty)}";
    }

    protected string GetComments(string indentation)
    {
        return $@"{(!XmlComments.IsEmpty() ? $@"{XmlComments.ToString(indentation)}
" : string.Empty)}";
    }
}

public static class CSharpDeclarationExtensions
{ 

    public static bool TryAddXmlDocComments<TImpl>( this CSharpDeclaration<TImpl> item, IElement element) where TImpl : CSharpDeclaration<TImpl>
    {
        if (element == null || string.IsNullOrWhiteSpace( element.Comment))
            return false;

        string comment = element.Comment;
        string formattedComment;

        if (comment.Contains("<summary>"))
        {
            formattedComment = comment;
        }
        else
        {
            formattedComment = string.Concat(Enumerable.Empty<string>()
                .Append("<summary>")
                .Concat(comment.Replace("\r\n", "\n").Split('\n'))
                .Append("</summary>")
                .Select(line => $"/// {line}{Environment.NewLine}"));
        }
        item.WithComments(formattedComment);
        return true;

    }
}
    