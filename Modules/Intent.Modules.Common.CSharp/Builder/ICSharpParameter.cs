using Intent.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.Builder
{
    public interface ICSharpParameter
    {
        string Type { get; }
        string Name { get; }
        string DefaultValue { get; }
        string XmlDocComment { get; }    }
}
