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
    public class ProjectItemTemplatePartialRegistrations : ModelTemplateRegistrationBase<FileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ProjectItemTemplatePartialRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ProjectItemTemplatePartialTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, FileTemplateModel model)
        {
            return new ProjectItemTemplatePartialTemplate(TemplateId, project, model);
        }

        public override IEnumerable<FileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.GetFileTemplates(application)
                .ToList();
        }
    }
}