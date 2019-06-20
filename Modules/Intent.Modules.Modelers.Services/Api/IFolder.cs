using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    public interface IFolder : IHasStereotypes
    {
        string Id { get; }

        string Name { get; }

        IFolder ParentFolder { get; }

        bool IsPackage { get; }
    }
}