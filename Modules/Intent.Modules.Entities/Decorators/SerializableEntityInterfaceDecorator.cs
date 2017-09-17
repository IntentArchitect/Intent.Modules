using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using System.Collections.Generic;

namespace Intent.Modules.Entities.Decorators
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