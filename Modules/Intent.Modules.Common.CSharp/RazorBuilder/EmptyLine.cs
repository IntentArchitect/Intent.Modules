#nullable enable
using System;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public class EmptyLine : RazorFileNodeBase<EmptyLine, IEmptyLine>, IEmptyLine
{
    public EmptyLine(IRazorFile file) : base(file)
    {
    }

    public override string GetText(string indentation)
    {
        return Environment.NewLine;
    }
}