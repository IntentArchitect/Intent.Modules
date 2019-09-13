using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePartial
{
    public class ProjectItemTemplatePartialRegistrations : ModelTemplateRegistrationBase<ITemplateDefinition>
    {
        private readonly IMetadataManager _metadataManager;

        public ProjectItemTemplatePartialRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ProjectItemTemplatePartialTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, ITemplateDefinition model)
        {
            return new ProjectItemTemplatePartialTemplate(TemplateId, project, model);
        }

        public override IEnumerable<ITemplateDefinition> GetModels(IApplication applicationManager)
        {
            return _metadataManager.GetFileTemplates(applicationManager)
                .ToList();
        }
    }
}