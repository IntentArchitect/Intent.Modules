using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public interface IFolder : IHasStereotypes
    {
        string Id { get; }

        string Name { get; }

        IFolder ParentFolder { get; }

        bool IsPackage { get; }
    }
}