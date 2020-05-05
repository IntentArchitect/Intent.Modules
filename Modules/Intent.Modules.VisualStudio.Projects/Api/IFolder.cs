using Intent.Metadata.Models;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public interface IFolder : IMetadataModel, IHasStereotypes
    {
        string Name { get; }

        IFolder ParentFolder { get; }
    }
}