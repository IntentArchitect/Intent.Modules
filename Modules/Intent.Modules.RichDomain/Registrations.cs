using Intent.Modules.RichDomain.Decorators;
using Intent.Modules.RichDomain.Templates.EntityBehaviour;
using Intent.Modules.RichDomain.Templates.EntityBehaviourBase;
using Intent.Modules.RichDomain.Templates.EntitySpecification;
using Intent.Modules.RichDomain.Templates.EntityState;
using Intent.Modules.RichDomain.Templates.EntityStateInterface;
using Intent.Modules.RichDomain.Templates.Enum;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Registrations;
using System.Linq;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.RichDomain
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            var entityModels = metaDataManager.GetMetaData<Class>(new MetaDataIdentifier("Entity")).Where(x => x.BoundedContext() == application.ApplicationName).ToList();
            var enumModels = metaDataManager.GetMetaData<EnumDefinition>(new MetaDataIdentifier("Enums")).Where(x => x.BoundedContext() == application.ApplicationName).ToList();

            if (entityModels.All(x => !x.IsEntity()))
            {
                Logging.Log.Debug("Intent.RichDomain - no entities marked with Aggregate Root or Entity stereotypes.");
                return;
            }

            foreach (var model in entityModels)
            {
                if (!model.IsEntity())
                {
                    continue;
                }
                RegisterTemplate(DomainEntityStateTemplate.Identifier, project => new DomainEntityStateTemplate(model, project));
                RegisterTemplate(DomainEntityStateInterfaceTemplate.Identifier, project => new DomainEntityStateInterfaceTemplate(model, project));
                RegisterTemplate(DomainEntityBehaviourTemplate.Identifier, project => new DomainEntityBehaviourTemplate(model, project));
                RegisterTemplate(DomainEntityBehaviourBaseTemplate.Identifier, project => new DomainEntityBehaviourBaseTemplate(model, project));
                if (model.IsAggregateRoot())
                {
                    RegisterTemplate(DomainEntitySpecificationTemplate.Identifier, project => new DomainEntitySpecificationTemplate(model, project));
                }
            }

            foreach (var enumModel in enumModels.Where(x => x.Stereotypes.All(s => s.Name != "CommonType")))
            {
                RegisterTemplate(DomainEnumTemplate.Identifier, project => new DomainEnumTemplate(enumModel, project));
            }

            RegisterDecorator<IDomainEntityStateDecorator>(EnforceConstraintsEntityStateDecorator.Identifier, new EnforceConstraintsEntityStateDecorator());
        }
    }
}
