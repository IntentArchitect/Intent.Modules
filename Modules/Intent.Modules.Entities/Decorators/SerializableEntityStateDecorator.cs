using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntity;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Decorators
{
    public class SerializableEntityStateDecorator : DomainEntityStateDecoratorBase
    {
        public const string Identifier = "Intent.Serializable.Entity.Decorator";

        public SerializableEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
        {
        }

        public override IEnumerable<string> DeclareUsings()
        {
            return new List<string>() { "System.Runtime.Serialization" };
        }

        public override string ClassAnnotations(ClassModel @class)
        {
            return "    [DataContract]";
        }

        public override string PropertyAnnotations(AttributeModel attribute)
        {
            return "        [DataMember]";
        }

        public override string PropertyAnnotations(AssociationEndModel associationEnd)
        {
            return "        [DataMember]";
        }

        public override string GetBaseClass(ClassModel @class)
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