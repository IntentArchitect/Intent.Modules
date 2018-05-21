using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.MetaData;

namespace Intent.Modules.Entities.Decorators
{
    public class DDDEntityInterfaceDecorator : AbstractDomainEntityInterfaceDecorator
    {
        public const string Id = "Intent.Entities.DDD.EntityInterfaceDecorator";

        public override string ConvertAttributeType(IAttribute attribute)
        {
            //var @namespace = attribute.Type.Stereotypes.GetProperty<string>("CommonType", "Namespace");
            //if (@namespace != null)
            //{
            //    return @namespace + "." + attribute.TypeName();
            //}
            var domainType = attribute.Stereotypes.GetProperty<string>("DomainType", "Type");
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
            return $@"
        {associationEnd.Type(prefix: "I", suffix: "", readOnly: true)} {associationEnd.Name()} {{ get; }}
";
        }

        public override bool CanWriteDefaultOperation(IOperation operation)
        {
            return !operation.HasStereotype("Command Operation");
        }
    }
}
