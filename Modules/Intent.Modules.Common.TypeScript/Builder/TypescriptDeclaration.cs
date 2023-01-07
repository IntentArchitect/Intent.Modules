using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.TypeScript.Builder;

public abstract class TypescriptDeclaration<TImpl> : TypescriptMetadataBase<TImpl>
    where TImpl : TypescriptDeclaration<TImpl>
{
    public List<TypescriptDecorator> Decorators { get; } = new();
    public TypescriptComments Comments { get; } = new();

    public TImpl AddDecorator(string name, Action<TypescriptDecorator> configure = null)
    {
        return AddDecorator(new TypescriptDecorator(name), configure);
    }

    public TImpl AddDecorator(TypescriptDecorator decorator, Action<TypescriptDecorator> configure = null)
    {
        Decorators.Add(decorator);
        configure?.Invoke(decorator);
        return (TImpl)this;
    }

    public TImpl WithComments(string comments)
    {
        Comments.AddStatements(comments);
        return (TImpl)this;
    }

    public TImpl WithComments(IEnumerable<string> comments)
    {
        Comments.AddStatements(comments);
        return (TImpl)this;
    }

    protected string GetDecorators(string indentation)
    {
        return $@"{(Decorators.Any() ? $@"{string.Join(@"
", Decorators.Select(x => x.GetText(indentation)))}
" : string.Empty)}";
    }

    protected string GetComments(string indentation)
    {
        return $@"{(!Comments.IsEmpty() ? $@"{Comments.ToString(indentation)}
" : string.Empty)}";
    }
}