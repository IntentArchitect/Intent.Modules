using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using IClass = Intent.Modelers.Domain.Api.IClass;

namespace Intent.Modules.Entities.Templates.DomainEntityState
{
    public abstract class DomainEntityStateDecoratorBase : DecoratorBase, ITemplateDecorator, IDeclareUsings, IAttibuteTypeConverter
    {
        public DomainEntityStateTemplate Template { get; internal set; }

        public virtual string ClassAnnotations(IClass @class) { return null; }
        public virtual string GetBaseClass(IClass @class) { return null; }
        public virtual string Constructors(IClass @class) { return null; }
        public virtual string BeforeProperties(IClass @class) { return null; }
        public virtual string PropertyBefore(IAttribute attribute) { return null; }
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

        public virtual bool CanWriteDefaultAttribute(IAttribute attribute) { return true; }
        public virtual bool CanWriteDefaultAssociation(IAssociationEnd association) { return true; }

        public virtual IEnumerable<string> GetInterfaces(IClass @class) { return new List<string>(); }

        public virtual IEnumerable<string> DeclareUsings() { return new List<string>(); }
    }
}