using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.EntityFramework.Repositories.Templates.Repository;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.EntityFramework.Repositories.Templates.DeleteVisitor
{
    [Description(DeleteVisitorTemplate.Identifier)]
    public class DeleteVisitorTemplateRegistration : ListModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public DeleteVisitorTemplateRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => DeleteVisitorTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IList<IClass> models)
        {
            return new DeleteVisitorTemplate(models, project);
        }

        public override IList<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application).ToList();
        }
    }
}
