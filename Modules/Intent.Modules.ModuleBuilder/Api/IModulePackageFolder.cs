using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModulePackageFolder : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        IElement Parent { get; }
        string FolderPath { get; }
    }
}