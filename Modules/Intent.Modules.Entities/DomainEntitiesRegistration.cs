using System.Linq;
using Inoxico.Automation.Plugins.Packages.Domain.Decorators;
using Inoxico.Automation.Plugins.Packages.Domain.Templates.DomainEntity;
using Inoxico.Automation.Plugins.Packages.Domain.Templates.DomainEntityBehaviour;
using Inoxico.Automation.Plugins.Packages.Domain.Templates.DomainEntityInterface;
using Intent.MetaModel.Domain;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Registrations;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;

namespace Inoxico.Automation.Plugins.Packages.Domain
{
    public class DomainEntitiesRegistration : OldProjectTemplateRegistration
    {
        private readonly IEventDispatcher _eventDispatcher;

        public DomainEntitiesRegistration(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public override void RegisterStuff(IApplication applicationManager, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<AbstractDomainEntityDecorator>(nameof(DDDEntityDecorator), new DDDEntityDecorator());
            RegisterDecorator<AbstractDomainEntityDecorator>(nameof(SerializableEntityDecorator), new SerializableEntityDecorator());
            RegisterDecorator<AbstractDomainEntityInterfaceDecorator>(nameof(DDDEntityInterfaceDecorator), new DDDEntityInterfaceDecorator());
            RegisterDecorator<AbstractDomainEntityInterfaceDecorator>(nameof(SerializableEntityInterfaceDecorator), new SerializableEntityInterfaceDecorator());

            var entityModels = metaDataManager.GetMetaData<Class>(new MetaDataType("DomainEntity")).Where(x => x.Application.Name == applicationManager.ApplicationName);
            foreach (var entityModel in entityModels)
            {
                if (entityModel.Stereotypes.All(x => x.Name != "Serializable"))
                {
                    continue;
                }

                RegisterTemplate(nameof(DomainEntityTemplate), project => new DomainEntityTemplate(entityModel, project));
                RegisterTemplate(nameof(DomainEntityInterfaceTemplate), project => new DomainEntityInterfaceTemplate(entityModel, project));
                if (entityModel.Stereotypes.Any(x => x.Name == "Behaviours"))
                {
                    RegisterTemplate(nameof(DomainEntityBehaviourTemplate),project => new DomainEntityBehaviourTemplate(entityModel, project));
                }
            }
        }
    }
}
