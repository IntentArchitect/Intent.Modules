using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using IAssociationEnd = Intent.Modelers.Domain.Api.IAssociationEnd;

namespace Intent.Modules.Entities.Templates.DomainEntityState
{
    public abstract class DomainEntityStateDecoratorBase : DecoratorBase, ITemplateDecorator, IDeclareUsings, IAttibuteTypeConverter
    {
        public DomainEntityStateTemplate Template { get;}

        protected DomainEntityStateDecoratorBase(DomainEntityStateTemplate template)
        {
            Template = template;
        }

        public virtual string ClassAnnotations(ClassModel @class) { return null; }
        public virtual string GetBaseClass(ClassModel @class) { return null; }
        public virtual string Constructors(ClassModel @class) { return null; }
        public virtual string BeforeProperties(ClassModel @class) { return null; }
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

        public virtual IEnumerable<string> GetInterfaces(ClassModel @class) { return new List<string>(); }

        public virtual IEnumerable<string> DeclareUsings() { return new List<string>(); }
    }
}