using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiTraitModel", Version = "1.0")]

namespace Intent.Modules.Common
{
    /// <summary>
    /// Indicates that the model can be used as a parameter or return type of a service operation.
    /// </summary>
    public interface IServiceArgumentModel : IElementWrapper, IMetadataModel
    {
    }
}