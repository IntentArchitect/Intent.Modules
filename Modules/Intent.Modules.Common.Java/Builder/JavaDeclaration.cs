using System;
using System.Linq;
using System.Collections.Generic;

namespace Intent.Modules.Common.Java.Builder;

public abstract class JavaDeclaration<TImpl> : JavaMetadataBase<TImpl>
    where TImpl : JavaDeclaration<TImpl>
{
    public IList<JavaAnnotation> Annotations { get; } = new List<JavaAnnotation>();
    public JavadocComments JavadocComments { get; } = new();

    public TImpl AddAnnotation(string name, Action<JavaAnnotation> configure = null)
    {
        return AddAnnotation(new JavaAnnotation(name), configure);
    }

    public TImpl AddAnnotation(JavaAnnotation annotation, Action<JavaAnnotation> configure = null)
    {
        Annotations.Add(annotation);
        configure?.Invoke(annotation);
        return (TImpl)this;
    }

    public TImpl WithComments(string javadocComments)
    {
        JavadocComments.AddStatements(javadocComments);
        return (TImpl)this;
    }

    public TImpl WithComments(IEnumerable<string> javadocComments)
    {
        JavadocComments.AddStatements(javadocComments);
        return (TImpl)this;
    }

    protected string GetAnnotations(string indentation)
    {
        return $@"{(Annotations.Any() ? $@"{string.Join(@"
", Annotations.Select(x => x.GetText(indentation)))}
" : string.Empty)}";
    }

    protected string GetComments(string indentation)
    {
        return $@"{(!JavadocComments.IsEmpty() ? $@"{JavadocComments.ToString(indentation)}
" : string.Empty)}";
    }
}