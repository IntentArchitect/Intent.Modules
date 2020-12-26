using System.Collections.Generic;

namespace Intent.Modules.Common.VisualStudio
{
    public interface IHasAssemblyDependencies
    {
        IEnumerable<IAssemblyReference> GetAssemblyDependencies();
    }
}
