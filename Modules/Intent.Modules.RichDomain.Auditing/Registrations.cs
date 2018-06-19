using Intent.Modules.RichDomain.Auditing.Decorators;
using Intent.Modules.RichDomain.EntityFramework.Templates.EFMapping;
using Intent.Modules.RichDomain.Templates.EntityBehaviour;
using Intent.Modules.RichDomain.Templates.EntityBehaviourBase;
using Intent.Modules.RichDomain.Templates.EntityState;
using Intent.Modules.RichDomain.Templates.EntityStateInterface;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.RichDomain.Auditing
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
