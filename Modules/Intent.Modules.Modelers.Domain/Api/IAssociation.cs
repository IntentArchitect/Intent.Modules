using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public interface IAssociation : IMetadataModel
    {
        IAssociationEnd SourceEnd { get; }

        IAssociationEnd TargetEnd { get; }

        AssociationType AssociationType { get; }

        string Comment { get; }

        bool IsSelfReference();
    }
}