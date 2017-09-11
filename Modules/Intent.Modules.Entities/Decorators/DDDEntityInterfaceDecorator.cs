using Intent.MetaModel.Domain;
using Intent.Packages.Entities.Templates;
using Intent.Packages.Entities.Templates.DomainEntityInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Packages.Entities.Decorators
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
