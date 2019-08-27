using System.Collections;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public interface IClass : IMetadataModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }

        bool IsAbstract { get; }

        IEnumerable<string> GenericTypes { get; }

        IClass ParentClass { get; }

        IEnumerable<IClass> ChildClasses { get; }

        bool IsMapped { get; }

        IElementMapping MappedClass { get; }

        IElementApplication Application { get; }

        IEnumerable<IAttribute> Attributes { get; }

        IEnumerable<IOperation> Operations { get; }

        IEnumerable<IAssociationEnd> AssociatedClasses { get; }

        IEnumerable<IAssociation> OwnedAssociations { get; }

        string Comment { get; }
    }
}