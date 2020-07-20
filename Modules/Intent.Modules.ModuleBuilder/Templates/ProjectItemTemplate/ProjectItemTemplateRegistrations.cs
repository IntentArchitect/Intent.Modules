using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplate
{
    public class ProjectItemTemplateRegistrations : ModelTemplateRegistrationBase<FileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ProjectItemTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ProjectItemTemplateTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, FileTemplateModel model)
        {
            return new ProjectItemTemplateTemplate(TemplateId, project, model);
        }

        public override IEnumerable<FileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetFileTemplateModels()
                .Where(x => x.GetFileSettings().TemplatingMethod().IsT4Template())
                .ToList();
        }
    }
}
