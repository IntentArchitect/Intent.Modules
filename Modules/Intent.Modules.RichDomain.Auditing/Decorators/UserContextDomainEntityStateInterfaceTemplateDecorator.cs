using Intent.MetaModel.UMLModel;
using Intent.Modules.RichDomain.Templates.EntityStateInterface;
using Intent.SoftwareFactory.MetaModels.UMLModel;

namespace Intent.Modules.RichDomain.Auditing.Decorators
{
    public class UserContextDomainEntityStateInterfaceTemplateDecorator : IDomainEntityStateInterfaceTemplateDecorator
    {
        public const string Identifier = "Intent.RichDomain.Auditing.EntityStateInterface";
        public string[] InterfaceProperties(Class @class)
        {
            if (!@class.IsAggregateRoot() || Utils.HasParentClassWhichIsAggregateRoot(@class))
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
            if (!@class.IsAggregateRoot() || Utils.HasParentClassWhichIsAggregateRoot(@class))
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