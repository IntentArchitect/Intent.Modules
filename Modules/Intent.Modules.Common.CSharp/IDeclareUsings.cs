using System.Collections.Generic;

namespace Intent.Modules.Common
{
    /// <summary>
    /// Signifies that this Template or Decorator introduces additional using clauses.
    /// </summary>
    public interface IDeclareUsings
    {
        /// <summary>
        /// The returned collection of namespaces that should be included in the using clauses
        /// section of this Template (or Decorator's Template). Do not include the "using"
        /// or semi-colon, just the namespace (EG: "System.Linq").
        /// </summary>
        IEnumerable<string> DeclareUsings();
    }
}