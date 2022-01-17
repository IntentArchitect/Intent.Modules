using System.Collections.Generic;

namespace Intent.Modules.Common.VisualStudio
{
    /// <summary>
    /// Used to specify that has dependencies which require adding &lt;FrameworkReference /&gt; elements to the containing.csproj.
    /// </summary>
    public interface IHasFrameworkDependencies
    {
        /// <summary>
        /// Used to specify that has dependencies which require adding &lt;FrameworkReference /&gt; elements to the containing.csproj.
        /// </summary>
        IEnumerable<string> GetFrameworkDependencies();
    }
}