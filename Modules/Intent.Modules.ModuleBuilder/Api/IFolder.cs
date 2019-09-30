using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IFolder : IMetadataModel, IHasStereotypes
    {
        string Name { get; }

        IFolder ParentFolder { get; }

        bool IsPackage { get; }
    }
}