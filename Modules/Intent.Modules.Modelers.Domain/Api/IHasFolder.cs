using Intent.Modules.Common.Types.Api;

namespace Intent.Modelers.Domain.Api
{
    public interface IHasFolder
    {
        FolderModel Folder { get; }
    }
}