#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

internal class RazorCodeBlock : CSharpClass, IRazorCodeBlock
{
    public RazorCodeBlock(IRazorFile file) : base("RazorClass", file)
    {
        RazorFile = file;
        Parent = file;
    }

    public IRazorFile RazorFile { get; }
    public IList<IRazorFileNode> ChildNodes { get; } = new List<IRazorFileNode>();
    public IRazorFileNode? Parent { get; set; }
    ICSharpTemplate IBuildsCSharpMembers.Template => File.Template;

    public void AddChildNode(IRazorFileNode node)
    {
        throw new InvalidOperationException($"Cannot add child razor nodes to a {nameof(RazorCodeBlock)}");
    }

    public void InsertChildNode(int index, IRazorFileNode node)
    {
        throw new InvalidOperationException($"Cannot add child razor nodes to a {nameof(RazorCodeBlock)}");
    }

    public override string GetText(string indentation)
    {
        return $@"{indentation}@code {{
{string.Join(@"
", Declarations.ConcatCode(indentation + "    "))}
{indentation}}}
";
    }
}