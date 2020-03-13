using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModulePackage : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        string IconUrl { get; }
        List<string> TargetModelers { get; }
    }
}