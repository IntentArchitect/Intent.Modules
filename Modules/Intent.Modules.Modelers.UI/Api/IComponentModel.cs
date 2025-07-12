using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiTraitModel", Version = "1.0")]

namespace Intent.Modules.Modelers.UI.Api
{
    /// <summary>
    /// Marks the element as a reusable UI component, representing a piece of the user interface with its own logic, parameters, and rendering behavior.
    /// </summary>
    public interface IComponentModel : IElementWrapper, IMetadataModel
    {
    }
}