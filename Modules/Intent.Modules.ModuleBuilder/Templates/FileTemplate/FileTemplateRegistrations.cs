using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.FileTemplate
{
    public class FileTemplateRegistrations : FilePerModelTemplateRegistration<FileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public FileTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => FileTemplateTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, FileTemplateModel model)
        {
            return new FileTemplateTemplate(TemplateId, project, model);
        }

        public override IEnumerable<FileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetFileTemplateModels()
                .Where(x => x.GetFileSettings().TemplatingMethod().IsT4Template())
                .ToList();
        }
    }
}
