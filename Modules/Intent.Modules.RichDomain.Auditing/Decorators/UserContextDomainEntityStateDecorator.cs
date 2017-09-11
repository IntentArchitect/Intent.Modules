using System.Collections.Generic;
using Intent.MetaModel.UMLModel;
using Intent.Packages.RichDomain.Templates.EntityState;
using Intent.SoftwareFactory.MetaModels.UMLModel;

namespace Intent.Packages.RichDomain.Auditing.Decorators
{
    public class UserContextDomainEntityStateDecorator : IDomainEntityStateDecorator
    {
        public const string Identifier = "Intent.RichDomain.Auditing.EntityState";
        public IEnumerable<string> DeclareUsings() => new string[0];

        public string ClassAnnotations(Class @class) => null;

        public string PropertyFieldAnnotations(UmlAttribute attribute) => null;

        public string PropertyAnnotations(UmlAttribute attribute) => null;

        public string PropertySetterBefore(UmlAttribute attribute) => null;

        public string PropertySetterAfter(UmlAttribute attribute) => null;

        public string ConstructorWithOrmLoadingParameter(Class @class)
        {
            if (!@class.IsAggregateRoot())
            {
                return null;
            }

            return @"
            CreatedBy = UserContext.Current.UserName;";
        }

        public string[] PublicProperties(Class @class)
        {
            if (!@class.IsAggregateRoot())
            {
                return new string[0];
            }

            return new[]
            {
                "        public string CreatedBy { get; set; }"
            };
        }
    }
}