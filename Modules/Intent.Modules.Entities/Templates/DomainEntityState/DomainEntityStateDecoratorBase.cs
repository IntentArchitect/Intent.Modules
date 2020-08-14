using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntityState
{
    public abstract class DomainEntityStateDecoratorBase : DecoratorBase, ITemplateDecorator, IDeclareUsings, IAttributeTypeConverter
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
        public virtual string PropertyBefore(AttributeModel attribute) { return null; }
        public virtual string PropertyFieldAnnotations(AttributeModel attribute) { return null; }
        public virtual string PropertyAnnotations(AttributeModel attribute) { return null; }
        public virtual string PropertySetterBefore(AttributeModel attribute) { return null; }
        public virtual string PropertySetterAfter(AttributeModel attribute) { return null; }
        public virtual string PropertyAnnotations(AssociationEndModel associationEnd) { return null; }
        public virtual string PropertySetterBefore(AssociationEndModel associationEnd) { return null; }
        public virtual string PropertySetterAfter(AssociationEndModel associationEnd) { return null; }
        public virtual string AssociationBefore(AssociationEndModel associationEnd) { return null; }
        public virtual string AssociationAfter(AssociationEndModel associationEnd) { return null; }
        public virtual string ConvertAttributeType(AttributeModel attribute) { return null; }

        public virtual bool CanWriteDefaultAttribute(AttributeModel attribute) { return true; }
        public virtual bool CanWriteDefaultAssociation(AssociationEndModel association) { return true; }

        public virtual IEnumerable<string> GetInterfaces(ClassModel @class) { return new List<string>(); }

        public virtual IEnumerable<string> DeclareUsings() { return new List<string>(); }
    }
}