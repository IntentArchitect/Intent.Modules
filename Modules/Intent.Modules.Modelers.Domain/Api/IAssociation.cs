using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public interface IAssociation : IMetaModel
    {
        IAssociationEnd SourceEnd { get; }

        IAssociationEnd TargetEnd { get; }

        AssociationType AssociationType { get; }

        string Comment { get; }
    }
}