using Intent.Metadata.Models;

namespace Intent.Modules.Common;

public interface IElementWrapper
{
    IElement InternalElement { get; }
}