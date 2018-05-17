using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    public abstract class AbstractDomainEntityDecorator : DecoratorBase, ITemplateDecorator, IDeclareUsings, IAttibuteTypeConverter
    {
        public virtual string ClassAnnotations(IClass @class) { return null; }
        public virtual string GetBaseClass(IClass @class) { return null; }
        public virtual string Constructors(IClass @class) { return null; }
        public virtual string BeforeProperties(IClass @class) { return null; }
        public virtual string PropertyFieldAnnotations(IAttribute attribute) { return null; }
        public virtual string PropertyAnnotations(IAttribute attribute) { return null; }
        public virtual string PropertySetterBefore(IAttribute attribute) { return null; }
        public virtual string PropertySetterAfter(IAttribute attribute) { return null; }
        public virtual string PropertyAnnotations(IAssociationEnd associationEnd) { return null; }
        public virtual string PropertySetterBefore(IAssociationEnd associationEnd) { return null; }
        public virtual string PropertySetterAfter(IAssociationEnd associationEnd) { return null; }
        public virtual string AssociationBefore(IAssociationEnd associationEnd) { return null; }
        public virtual string AssociationAfter(IAssociationEnd associationEnd) { return null; }
        public virtual string ConvertAttributeType(IAttribute attribute) { return null; }

        public virtual bool CanWriteAttribute(IAttribute attribute) { return true; }
        public virtual bool CanWriteAssociation(IAssociationEnd association) { return true; }

        public virtual IEnumerable<string> GetInterfaces(IClass @class) { return new List<string>(); }

        public virtual IEnumerable<string> DeclareUsings() { return new List<string>(); }
    }
}