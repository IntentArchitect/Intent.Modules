using System.Linq;
using Intent.MetaModel.UMLModel;
using Intent.Packages.RichDomain.Decorators;
using Intent.Packages.RichDomain.Templates.EntityBehaviour;
using Intent.Packages.RichDomain.Templates.EntityBehaviourBase;
using Intent.Packages.RichDomain.Templates.EntitySpecification;
using Intent.Packages.RichDomain.Templates.EntityState;
using Intent.Packages.RichDomain.Templates.EntityStateInterface;
using Intent.Packages.RichDomain.Templates.Enum;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.RichDomain
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var entityModels = metaDataManager.GetMetaData<Class>(new MetaDataType("Entity")).Where(x => x.BoundedContext() == application.ApplicationName).ToList();
            var enumModels = metaDataManager.GetMetaData<EnumDefinition>(new MetaDataType("Enums")).Where(x => x.BoundedContext() == application.ApplicationName).ToList();

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
