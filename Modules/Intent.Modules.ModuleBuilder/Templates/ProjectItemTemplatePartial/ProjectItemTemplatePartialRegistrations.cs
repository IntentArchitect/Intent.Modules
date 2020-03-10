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
    public class ProjectItemTemplatePartialRegistrations : ModelTemplateRegistrationBase<IFileTemplate>
    {
        private readonly IMetadataManager _metadataManager;

        public ProjectItemTemplatePartialRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ProjectItemTemplatePartialTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IFileTemplate model)
        {
            return new ProjectItemTemplatePartialTemplate(TemplateId, project, model);
        }

        public override IEnumerable<IFileTemplate> GetModels(IApplication application)
        {
            return _metadataManager.GetFileTemplates(application)
                .ToList();
        }
    }
}