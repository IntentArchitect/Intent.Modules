using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.Packages.Entities.Templates.DomainEntityInterface;
using Intent.Packages.Entities.Templates;

namespace Intent.Packages.Entities.Decorators
{
    public class SerializableEntityInterfaceDecorator : AbstractDomainEntityInterfaceDecorator
    {
        public const string Id = "Intent.Serializable.Entity.Interfaces.Decorator";

        public override IEnumerable<string> GetInterfaces(IClass @class)
        {
            var baseClass = @class.Stereotypes.GetProperty<string>("Serializable", "BaseType");
            if (baseClass != null)
            {
                return new[]
                {
                    $"I{baseClass}",
                };
            }

            return base.GetInterfaces(@class);
        }
    }
}