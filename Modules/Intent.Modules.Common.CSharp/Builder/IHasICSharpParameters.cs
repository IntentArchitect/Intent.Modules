using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder
{
    public interface IHasICSharpParameters
    {
        public IEnumerable<ICSharpParameter> Parameters { get; }
    }
}
