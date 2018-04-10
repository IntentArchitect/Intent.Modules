using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Modules.EntityFramework.Templates.DeleteVisitor;
using Intent.Modules.EntityFramework.Templates.EFMapping;
using Intent.Modules.EntityFramework.Templates.Repository;
using Intent.Modules.EntityFramework.Templates.RepositoryContract;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Registrations;
using System.Linq;
using Intent.Modules.RichDomain;

namespace Intent.Modules.EntityFramework
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var models = metaDataManager.GetMetaData<Class>(new MetaDataIdentifier("Entity")).Where(x => x.BoundedContext() == application.ApplicationName).ToList();

            RegisterTemplate(DbContextTemplate.Identifier, project => new DbContextTemplate(models, project, application.EventDispatcher));
            RegisterTemplate(DeleteVisitorTemplate.Identifier, project => new DeleteVisitorTemplate(models, project));
            foreach (var model in models)
            {
                if (!model.IsEntity())
                {
                    continue;
                }
                RegisterTemplate(EFMappingTemplate.Identifier, project => new EFMappingTemplate(model, project));
                if (model.IsAggregateRoot())
                {
                    RegisterTemplate(RepositoryContractTemplate.Identifier, project => new RepositoryContractTemplate(model, project));
                    RegisterTemplate(RepositoryTemplate.Identifier, project => new RepositoryTemplate(model, project));
                }
            }

        }
    }
}
