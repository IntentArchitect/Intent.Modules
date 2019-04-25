using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public interface IClass : IMetaModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }

        IEnumerable<string> GenericTypes { get; }

        IClass ParentClass { get; }

        bool IsMapped { get; }

        IClassMapping MappedClass { get; }

        IApplication Application { get; }

        IEnumerable<IAttribute> Attributes { get; }

        IEnumerable<IOperation> Operations { get; }

        IEnumerable<IAssociationEnd> AssociatedClasses { get; }

        IEnumerable<IAssociation> OwnedAssociations { get; }

        string Comment { get; }
    }
}