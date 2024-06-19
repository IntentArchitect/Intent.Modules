using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IRazorFileNode : ICSharpCodeContext
{
    string GetText(string indentation);
    void AddChildNode(IRazorFileNode node);
    void InsertChildNode(int index, IRazorFileNode node);
    IList<IRazorFileNode> ChildNodes { get; }
    IRazorFileNode Parent { get; internal set; }
}