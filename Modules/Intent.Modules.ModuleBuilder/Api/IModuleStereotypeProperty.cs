using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModuleStereotypeProperty : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        string ControlType { get; }
        string OptionsSource { get; }
        string Comment { get; }
        IList<string> ValueOptions { get; }
        IList<string> LookupTypes { get; }
        string TargetPropertyId { get; }
        string DefaultValue { get; }
        string IsActiveFunction { get; }
    }
}