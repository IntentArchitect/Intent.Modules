using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.Entities.Templates.DomainEntityInterface
{
    public abstract class DomainEntityInterfaceDecoratorBase : ITemplateDecorator, IDeclareUsings, IAttibuteTypeConverter, IPriorityDecorator
    {
        public DomainEntityInterfaceTemplate Template { get; internal set; }

        public virtual IEnumerable<string> DeclareUsings() { return new List<string>(); }

        public virtual IEnumerable<string> GetInterfaces(IClass @class) { return new List<string>(); }

        public virtual string InterfaceAnnotations(IClass @class) { return null; }

        public virtual string PropertyBefore(IAttribute attribute) { return null; }

        public virtual string PropertyAnnotations(IAttribute attribute) { return null; }

        public virtual string PropertyBefore(IAssociationEnd associationEnd) { return null; }

        public virtual string PropertyAnnotations(IAssociationEnd associationEnd) { return null; }

        public virtual string AttributeAccessors(IAttribute attribute) { return null; }

        public virtual string AssociationAccessors(IAssociationEnd associationEnd) { return null; }

        public virtual string ConvertAttributeType(IAttribute attribute) { return null; }

        public virtual bool CanWriteDefaultAttribute(IAttribute attribute) { return true; }
        public virtual bool CanWriteDefaultAssociation(IAssociationEnd association) { return true; }
        public virtual bool CanWriteDefaultOperation(IOperation operation) { return true; }

        public int Priority { get; set; } = 0;
    }
}