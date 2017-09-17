using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;

namespace Intent.Modules.Entities.Decorators
{
    public class DDDEntityInterfaceDecorator : AbstractDomainEntityInterfaceDecorator
    {
        public const string Id = "Intent.DDD.Entity.Interfaces.Decorator";

        public override string ConvertAttributeType(IAttribute attribute)
        {
            var @namespace = attribute.Type.Stereotypes.GetProperty<string>("CommonType", "Namespace");
            if (@namespace != null)
            {
                return @namespace + "." + attribute.TypeName();
            }
            var domainType = attribute.Stereotypes.GetProperty<string>("DomainType", "Type");
            if (domainType != null)
            {
                return domainType;
            }
            return base.ConvertAttributeType(attribute);
        }
    }
}
