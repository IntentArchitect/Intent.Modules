using System;
using System.Collections.Generic;
using Intent.Engine;

namespace Intent.Modules.Common.Kotlin.Templates
{
    public interface IDeclareImports
    {
        IEnumerable<string> DeclareImports();
    }
}