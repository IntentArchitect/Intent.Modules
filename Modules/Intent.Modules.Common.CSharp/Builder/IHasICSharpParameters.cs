using Intent.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.CSharp.Builder
{
    public interface IHasICSharpParameters
    {
        public IEnumerable<ICSharpParameter> Parameters { get; }
    }
}
