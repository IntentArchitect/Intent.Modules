using System.Linq;
using Intent.MetaModel.UMLModel;
using Intent.Packages.EntityFramework.Templates.EFMapping;
using Intent.Packages.RichDomain.Auditing.Decorators;
using Intent.Packages.RichDomain.Templates.EntityBehaviour;
using Intent.Packages.RichDomain.Templates.EntityBehaviourBase;
using Intent.Packages.RichDomain.Templates.EntityState;
using Intent.Packages.RichDomain.Templates.EntityStateInterface;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.RichDomain.Auditing
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IDomainBehaviourDecorator>(UserContextDomainBehaviourDecorator.Identifier, new UserContextDomainBehaviourDecorator());
            RegisterDecorator<IDomainEntityBehaviourBaseDecorator>(UserContextDomainEntityBehaviourBaseDecorator.Identifier, new UserContextDomainEntityBehaviourBaseDecorator());
            RegisterDecorator<IDomainEntityStateDecorator>(UserContextDomainEntityStateDecorator.Identifier, new UserContextDomainEntityStateDecorator());
            RegisterDecorator<IDomainEntityStateInterfaceTemplateDecorator>(UserContextDomainEntityStateInterfaceTemplateDecorator.Identifier, new UserContextDomainEntityStateInterfaceTemplateDecorator());
            RegisterDecorator<IEFMappingTemplateDecorator>(UserContextEFMappingTemplateDecorator.Identifier, new UserContextEFMappingTemplateDecorator());
        }
    }
}
