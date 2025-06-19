using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiTraitModel", Version = "1.0")]

namespace Intent.Modules.Common
{
    /// <summary>
    /// Indicates that this element can be invoked from a [Processing Handler] (e.g. Commands, Queries,  Operations on Services, etc).
    /// </summary>
    public interface IInvokableModel : IElementWrapper, IMetadataModel
    {
    }
}