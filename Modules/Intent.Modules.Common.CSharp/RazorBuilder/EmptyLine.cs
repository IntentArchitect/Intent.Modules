using System;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public class EmptyLine : RazorFileNodeBase<HtmlElement>
{
    public EmptyLine(RazorFile file) : base(file)
    {
    }
    public override string GetText(string indentation)
    {
        return Environment.NewLine;
    }
}