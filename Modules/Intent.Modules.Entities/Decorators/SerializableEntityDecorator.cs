using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.Packages.Entities.Templates.DomainEntity;
using Intent.Packages.Entities.Templates;

namespace Intent.Packages.Entities.Decorators
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