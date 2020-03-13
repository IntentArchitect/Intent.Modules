using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

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