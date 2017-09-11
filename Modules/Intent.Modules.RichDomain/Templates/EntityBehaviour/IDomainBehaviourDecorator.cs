using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.RichDomain.Templates.EntityBehaviour
{
    public interface IDomainBehaviourDecorator : ITemplateDecorator
    {
        string OperationStart(Class model);
    }
}
