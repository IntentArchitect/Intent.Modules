using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplate
{
    public class ProjectItemTemplateRegistrations : ModelTemplateRegistrationBase<ITemplateDefinition>
    {
        private readonly IMetadataManager _metadataManager;

        public ProjectItemTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ProjectItemTemplateTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, ITemplateDefinition model)
        {
            return new ProjectItemTemplateTemplate(TemplateId, project, model);
        }

        public override IEnumerable<ITemplateDefinition> GetModels(IApplication applicationManager)
        {
            return _metadataManager.GetFileTemplates(applicationManager)
                .ToList();
        }
    }
}
