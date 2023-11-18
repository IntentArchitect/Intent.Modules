using Intent.Metadata.Models;
using System.Collections.Generic;

namespace Intent.Modules.Common;

/// <summary>
/// Indicates that the model represents a processing point in the application. This can then be used to discover interactions
/// that have been modeled against this processing point.
/// </summary>
public interface IProcessingHandlerModel : IElementWrapper, IMetadataModel
{

}