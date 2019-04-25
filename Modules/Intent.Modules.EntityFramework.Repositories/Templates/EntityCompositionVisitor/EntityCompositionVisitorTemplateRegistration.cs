using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFramework.Repositories.Templates.Repository;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.EntityFramework.Repositories.Templates.EntityCompositionVisitor
{
    // Disabled for now as this is only needed in more complex use cases (e.g. using the DbContext for Auditing purposes)
    [Description(EntityCompositionVisitorTemplate.Identifier)]
    public class EntityCompositionVisitorTemplateRegistration : ListModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public EntityCompositionVisitorTemplateRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => EntityCompositionVisitorTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IList<IClass> models)
        {
            return new EntityCompositionVisitorTemplate(models, project);
        }

        public override IList<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application).ToList();
        }
    }
}
