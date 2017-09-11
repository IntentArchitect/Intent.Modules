using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.Entities.Templates.DomainEntityInterface
{
    public abstract class AbstractDomainEntityInterfaceDecorator : ITemplateDecorator, IDeclareUsings, IAttibuteTypeConverter
    {
        public virtual IEnumerable<string> DeclareUsings()
        {
            return new List<string>();
        }

        public virtual IEnumerable<string> GetInterfaces(IClass @class)
        {
            return new List<string>();
        }

        public virtual string InterfaceAnnotations(IClass @class)
        {
            return null;
        }

        public virtual string PropertyAnnotations(IAttribute attribute)
        {
            return null;
        }

        public virtual string PropertyAnnotations(IAssociationEnd associationEnd)
        {
            return null;
        }

        public virtual string ConvertAttributeType(IAttribute attribute)
        {
            return null;
        }
    }
}