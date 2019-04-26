using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntity;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Entities.Templates.DomainEntityState;
using IClass = Intent.Modelers.Domain.Api.IClass;

namespace Intent.Modules.Entities.Decorators
{
    public class SerializableEntityStateDecorator : DomainEntityStateDecoratorBase
    {
        public const string Identifier = "Intent.Serializable.Entity.Decorator";

        public override IEnumerable<string> DeclareUsings()
        {
            return new List<string>() { "System.Runtime.Serialization" };
        }

        public override string ClassAnnotations(IClass @class)
        {
            return "    [DataContract]";
        }

        public override string PropertyAnnotations(IAttribute attribute)
        {
            return "        [DataMember]";
        }

        public override string PropertyAnnotations(IAssociationEnd associationEnd)
        {
            return "        [DataMember]";
        }

        public override string GetBaseClass(IClass @class)
        {
            var baseClass = @class.GetStereotypeProperty<string>("Serializable", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            return base.GetBaseClass(@class);
        }
    }
}