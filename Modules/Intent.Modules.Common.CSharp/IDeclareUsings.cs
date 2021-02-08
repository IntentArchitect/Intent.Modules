using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Intent.Engine;

namespace Intent.Modules.Common
{
    /// <summary>
    /// Signifies that this Template or Decorator introduces additional using clauses.
    /// </summary>
    public interface IDeclareUsings
    {
        /// <summary>
        /// Returns collection of namespaces that should be included in the using clauses section of this Template (or Decorator's Template).
        /// Do not include the "using" or semi-colon, just the namespace.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> DeclareUsings();
    }
}