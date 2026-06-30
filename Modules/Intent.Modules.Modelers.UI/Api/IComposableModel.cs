using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiTraitModel", Version = "1.0")]

namespace Intent.Modules.Modelers.UI.Api
{
    /// <summary>
    /// Auto-managed marker. Applied to Components that are not Pages or Dialogs. Used by Composition target filtering.
    /// </summary>
    public interface IComposableModel : IElementWrapper, IMetadataModel
    {
    }
}
