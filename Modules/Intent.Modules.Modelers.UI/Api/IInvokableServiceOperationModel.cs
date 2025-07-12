using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiTraitModel", Version = "1.0")]

namespace Intent.Modules.Modelers.UI.Api
{
    /// <summary>
    /// Marks the operation as directly callable.
    /// </summary>
    public interface IInvokableServiceOperationModel : IElementWrapper, IMetadataModel
    {
    }
}