﻿using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.RazorBuilder
{
    public interface IRazorFileTemplate : ICSharpTemplate
    {
        RazorFile RazorFile { get; }
    }
}