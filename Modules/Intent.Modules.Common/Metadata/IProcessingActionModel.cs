using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiTraitModel", Version = "1.0")]

namespace Intent.Modules.Common
{
    /// <summary>
    /// Indicates that this element represents a processing action within a <see cref="IProcessingHandlerModel"/>
    /// </summary>
    public interface IProcessingActionModel : IElementWrapper, IMetadataModel
    {
    }
}