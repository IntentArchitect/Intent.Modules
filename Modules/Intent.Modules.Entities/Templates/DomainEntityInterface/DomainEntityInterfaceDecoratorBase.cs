using Intent.Templates;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntityInterface
{
    public abstract class DomainEntityInterfaceDecoratorBase : DecoratorBase, ITemplateDecorator, IDeclareUsings, IAttributeTypeConverter
    {
        protected DomainEntityInterfaceDecoratorBase(DomainEntityInterfaceTemplate template)
        {
            Template = template;
        }

        public DomainEntityInterfaceTemplate Template { get; private set; }

        public virtual IEnumerable<string> DeclareUsings() { return new List<string>(); }

        public virtual IEnumerable<string> GetInterfaces(ClassModel @class) { return new List<string>(); }

        public virtual string InterfaceAnnotations(ClassModel @class) { return null; }

        public virtual string BeforeProperties(ClassModel @class) { return null; }

        public virtual string PropertyBefore(AttributeModel attribute) { return null; }

        public virtual string PropertyAnnotations(AttributeModel attribute) { return null; }

        public virtual string PropertyBefore(AssociationEndModel associationEnd) { return null; }

        public virtual string PropertyAnnotations(AssociationEndModel associationEnd) { return null; }

        public virtual string AttributeAccessors(AttributeModel attribute) { return null; }

        public virtual string AssociationAccessors(AssociationEndModel associationEnd) { return null; }

        public virtual string ConvertAttributeType(AttributeModel attribute) { return null; }

        public virtual bool CanWriteDefaultAttribute(AttributeModel attribute) { return true; }
        public virtual bool CanWriteDefaultAssociation(AssociationEndModel association) { return true; }
        public virtual bool CanWriteDefaultOperation(OperationModel operation) { return true; }
    }
}