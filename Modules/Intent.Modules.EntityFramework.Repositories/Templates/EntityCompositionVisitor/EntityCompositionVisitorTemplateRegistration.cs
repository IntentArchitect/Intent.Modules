using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFramework.Repositories.Templates.Repository;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Modelers.Domain;
using Intent.Registrations;
using Intent.Templates;


namespace Intent.Modules.EntityFramework.Repositories.Templates.EntityCompositionVisitor
{
    // Disabled for now as this is only needed in more complex use cases (e.g. using the DbContext for Auditing purposes)
    [Description(EntityCompositionVisitorTemplate.Identifier)]
    public class EntityCompositionVisitorTemplateRegistration : ListModelTemplateRegistrationBase<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public EntityCompositionVisitorTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => EntityCompositionVisitorTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IList<ClassModel> models)
        {
            return new EntityCompositionVisitorTemplate(models, project);
        }

        public override IList<ClassModel> GetModels(Engine.IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels().ToList();
        }
    }
}
