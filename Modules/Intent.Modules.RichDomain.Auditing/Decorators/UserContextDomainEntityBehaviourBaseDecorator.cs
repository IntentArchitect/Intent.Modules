using Intent.Modules.RichDomain.Templates.EntityBehaviourBase;
using Intent.SoftwareFactory.MetaModels.UMLModel;

namespace Intent.Modules.RichDomain.Auditing.Decorators
{
    public class UserContextDomainEntityBehaviourBaseDecorator : IDomainEntityBehaviourBaseDecorator
    {
        public const string Identifier = "Intent.RichDomain.Auditing.EntityBehaviourBase";
        public string[] PublicProperties(Class @class)
        {
            if (!@class.IsAggregateRoot() || Utils.HasParentClassWhichIsAggregateRoot(@class))
            {
                return new string[0];
            }

            return new[]
            {
                @"
        public string CreatedBy
        {
            get { return _state.CreatedBy; }
            set { _state.CreatedBy = value; }
        }
"
            };
        }
    }
}