using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.Templates;

namespace Intent.Modules.RichDomain.Templates.EntityBehaviour
{
    public interface IDomainBehaviourDecorator : ITemplateDecorator
    {
        string OperationStart(Class @class);
    }
}
