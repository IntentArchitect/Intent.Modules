using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using System.Collections.Generic;
using Intent.Modules.Common;

namespace Intent.Modules.Entities.Decorators
{
    public class SerializableEntityInterfaceDecorator : DomainEntityInterfaceDecoratorBase
    {
        public const string Identifier = "Intent.Serializable.Entity.Interfaces.Decorator";

        public override IEnumerable<string> GetInterfaces(IClass @class)
        {
            var baseClass = @class.GetStereotypeProperty<string>("Serializable", "BaseType");
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