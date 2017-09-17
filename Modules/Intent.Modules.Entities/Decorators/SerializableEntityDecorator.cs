using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntity;
using System.Collections.Generic;

namespace Intent.Modules.Entities.Decorators
{
    public class SerializableEntityDecorator : AbstractDomainEntityDecorator
    {
        public const string Id = "Intent.Serializable.Entity.Decorator";

        public override IEnumerable<string> DeclareUsings()
        {
            return new List<string>() { "using System.Runtime.Serialization;" };
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
            var baseClass = @class.Stereotypes.GetProperty<string>("Serializable", "BaseType");
            if (baseClass != null)
            {
                return baseClass;
            }
            return base.GetBaseClass(@class);
        }
    }
}