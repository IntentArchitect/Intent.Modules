using Intent.MetaModel.UMLModel;
using Intent.Packages.RichDomain.Templates.EntityStateInterface;
using Intent.SoftwareFactory.MetaModels.UMLModel;

namespace Intent.Packages.RichDomain.Auditing.Decorators
{
    public class UserContextDomainEntityStateInterfaceTemplateDecorator : IDomainEntityStateInterfaceTemplateDecorator
    {
        public const string Identifier = "Intent.RichDomain.Auditing.EntityStateInterface";
        public string[] InterfaceProperties(Class @class)
        {
            if (!@class.IsAggregateRoot())
            {
                return new string[0];
            }

            return new[]
            {
                @"        string CreatedBy { get; }"
            };
        }

        public string[] ImplementationPartialProperties(Class @class, string readOnlyInterfaceName)
        {
            if (!@class.IsAggregateRoot())
            {
                return new string[0];
            }

            return new[]
            {
                $@"        string {readOnlyInterfaceName}.CreatedBy
        {{
            get {{ return CreatedBy; }}
        }}"
            };
        }
    }
}