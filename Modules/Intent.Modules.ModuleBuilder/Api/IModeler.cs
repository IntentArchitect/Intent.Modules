using System.Collections;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModeler : IMetadataModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }
        IEnumerable<IModelerModelType> ModelTypes { get; }
        string ModuleDependency { get; }
        string ModuleVersion { get; }
        string NuGetDependency { get; }
        string NuGetVersion { get; }
    }
}