using Intent.Modules.RichDomain.Templates.EntityBehaviour;
using Intent.SoftwareFactory.MetaModels.UMLModel;

namespace Intent.Modules.RichDomain.Auditing.Decorators
{
    public class UserContextDomainBehaviourDecorator : IDomainBehaviourDecorator
    {
        public const string Identifier = "Intent.RichDomain.Auditing.DomainBehaviour";
        public string OperationStart(Class @class)
        {
            if (!@class.IsAggregateRoot() && !Utils.HasParentClassWhichIsAggregateRoot(@class))
            {
                return null;
            }

            return @"
            UpdateDateTime = DateTime.UtcNow;
            UpdatedBy = UserContext.Current.UserName;";
        }
    }
}