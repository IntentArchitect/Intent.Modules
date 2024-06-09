using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public abstract class RazorFileNodeBase<T> : CSharpMetadataBase<T>, IRazorFileNode 
    where T : RazorFileNodeBase<T>
{
    protected RazorFileNodeBase(RazorFile file)
    {
        File = file;
        RazorFile = file;
    }

    public RazorFile RazorFile { get; protected set; }

    public IList<IRazorFileNode> ChildNodes { get; } = new List<IRazorFileNode>();

    public T AddHtmlElement(string name, Action<HtmlElement> configure = null)
    {
        var htmlElement = new HtmlElement(name, RazorFile);
        AddChildNode(htmlElement);
        configure?.Invoke(htmlElement);
        return (T)this;
    }

    public T AddHtmlElement(HtmlElement htmlElement)
    {
        AddChildNode(htmlElement);
        return (T)this;
    }

    public T AddEmptyLine()
    {
        AddChildNode(new EmptyLine(RazorFile));
        return (T)this;
    }

    public T AddCodeBlock(CSharpStatement expression, Action<RazorCodeDirective> configure = null)
    {
        var razorCodeBlock = new RazorCodeDirective(expression, RazorFile);
        AddChildNode(razorCodeBlock);
        configure?.Invoke(razorCodeBlock);
        return (T)this;
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

    public new IRazorFileNode Parent { get; set; }
}