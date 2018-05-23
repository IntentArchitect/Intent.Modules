using System.Collections;
using Intent.MetaModel.Domain;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.MetaData;

namespace Intent.Modules.Entities.Decorators
{
    public class DDDEntityInterfaceDecorator : DomainEntityInterfaceDecoratorBase
    {
        public const string Id = "Intent.Entities.DDD.EntityInterfaceDecorator";

        public override string ConvertAttributeType(IAttribute attribute)
        {
            //var @namespace = attribute.Type.GetStereotypeProperty<string>("CommonType", "Namespace");
            //if (@namespace != null)
            //{
            //    return @namespace + "." + attribute.TypeName();
            //}
            var domainType = attribute.GetStereotypeProperty<string>("DomainType", "Type");
            if (domainType != null)
            {
                return domainType;
            }
            return base.ConvertAttributeType(attribute);
        }

        public override string AttributeAccessors(IAttribute attribute)
        {
            return "get;";
        }

        public override bool CanWriteDefaultAssociation(IAssociationEnd association)
        {
            return false;
        }

        public override string PropertyBefore(IAssociationEnd associationEnd)
        {
            if (!associationEnd.IsNavigable)
            {
                return base.PropertyBefore(associationEnd);
            }
            var t = ClassTypeSource.InProject(Template.Project, DomainEntityInterfaceTemplate.Identifier, nameof(IEnumerable));
            return $@"
        {t.GetClassType(associationEnd)} {associationEnd.Name()} {{ get; }}
";
        }

        public override bool CanWriteDefaultOperation(IOperation operation)
        {
            return !operation.HasStereotype("Command Operation");
        }
    }
}
