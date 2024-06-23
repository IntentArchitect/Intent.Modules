#nullable enable
using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IRazorFileNodeBase<out TInterface> : IRazorFileNode
    where TInterface : IRazorFileNodeBase<TInterface>
{
    public IRazorFile RazorFile { get; }
    public IList<IRazorFileNode> ChildNodes { get; }
    public TInterface AddHtmlElement(string name, Action<IHtmlElement>? configure = null);
    public TInterface AddHtmlElement(IHtmlElement htmlElement);
    public TInterface AddEmptyLine();
    public TInterface AddCodeBlock(ICSharpStatement expression, Action<IRazorCodeDirective>? configure = null);
    public TInterface AddCodeBlock(string expression, Action<IRazorCodeDirective>? configure = null) => AddCodeBlock(new CSharpStatement(expression), configure);
    TInterface AddAbove(IRazorFileNode node);
    TInterface AddAbove(params IRazorFileNode[] nodes);
    TInterface AddBelow(IRazorFileNode node);
    TInterface AddBelow(params IRazorFileNode[] nodes);
}