using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiTraitModel", Version = "1.0")]

namespace Intent.Modules.Common;

/// <summary>
/// Indicates that the model represents a processing point in the application. This can then be used to discover interactions that have been modeled against this processing point.
/// </summary>
public interface IProcessingHandlerModel : IElementWrapper, IMetadataModel
{

}