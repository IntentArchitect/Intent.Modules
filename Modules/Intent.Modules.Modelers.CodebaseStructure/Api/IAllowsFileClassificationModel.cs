using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiTraitModel", Version = "1.0")]

namespace Intent.Modules.Modelers.CodebaseStructure.Api
{
    /// <summary>
    /// Opts an element type in to the File Classification stereotype. Apply it to an Element Settings, Package Settings or extension in a designer you own. It is never applied in an application's own model.
    /// </summary>
    public interface IAllowsFileClassificationModel : IElementWrapper, IMetadataModel
    {
    }
}