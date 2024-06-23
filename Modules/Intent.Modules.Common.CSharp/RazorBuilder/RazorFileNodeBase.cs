#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public abstract class RazorFileNodeBase<TConcrete, TInterface> : CSharpMetadataBase<TConcrete>, IRazorFileNodeBase<TInterface>
    where TConcrete : RazorFileNodeBase<TConcrete, TInterface>, TInterface
    where TInterface : class, IRazorFileNodeBase<TInterface>
{
    protected RazorFileNodeBase(IRazorFile file)
    {
        File = file;
        RazorFile = file;
    }

    public IRazorFile RazorFile { get; protected set; }

    public IList<IRazorFileNode> ChildNodes { get; } = new List<IRazorFileNode>();

    public TInterface AddHtmlElement(string name, Action<IHtmlElement>? configure = null)
    {
        var htmlElement = new HtmlElement(name, RazorFile);
        AddChildNode(htmlElement);
        configure?.Invoke(htmlElement);
        return this as TInterface ?? throw new InvalidOperationException();
    }

    public TInterface AddHtmlElement(IHtmlElement htmlElement)
    {
        AddChildNode(htmlElement);
        return (TConcrete)this;
    }

    public TInterface AddEmptyLine()
    {
        AddChildNode(new EmptyLine(RazorFile));
        return (TConcrete)this;
    }

    public TInterface AddCodeBlock(string expression, Action<IRazorCodeDirective>? configure = null) => AddCodeBlock(new CSharpStatement(expression), configure);
    public TInterface AddCodeBlock(ICSharpStatement expression, Action<IRazorCodeDirective>? configure = null)
    {
        var razorCodeBlock = new RazorCodeDirective(expression, RazorFile);
        AddChildNode(razorCodeBlock);
        configure?.Invoke(razorCodeBlock);
        return (TConcrete)this;
    }

    public abstract string GetText(string indentation);


    public void AddChildNode(IRazorFileNode node)
    {
        InsertChildNode(ChildNodes.Count, node);
    }

    public void InsertChildNode(int index, IRazorFileNode node)
    {
        node.Parent = this;
        ChildNodes.Insert(index, node);
    }

    public TInterface AddAbove(IRazorFileNode node)
    {
        Parent.InsertChildNode(Parent.ChildNodes.IndexOf(this), node);
        return (TConcrete)this;
    }

    public TInterface AddAbove(params IRazorFileNode[] nodes)
    {
        foreach (var node in nodes)
        {
            AddAbove(node);
        }
        return (TConcrete)this;
    }

    public TInterface AddBelow(IRazorFileNode node)
    {
        Parent.InsertChildNode(Parent.ChildNodes.IndexOf(this) + 1, node);
        return (TConcrete)this;
    }

    public TInterface AddBelow(params IRazorFileNode[] nodes)
    {
        foreach (var node in nodes.Reverse())
        {
            AddBelow(node);
        }
        return (TConcrete)this;
    }

    public new IRazorFileNode? Parent { get; set; }
}