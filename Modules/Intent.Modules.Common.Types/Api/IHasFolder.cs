using Intent.Metadata.Models;

namespace Intent.Modules.Common.Types.Api
{
    public interface IFolder : IHasStereotypes
    {
        string Name { get; }
    }

    public interface IHasFolder
    {
        FolderModel Folder { get; }
    }

    public interface IHasFolder<out TFolder>
        where TFolder: IFolder
    {
        TFolder Folder { get; }
    }
}