using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.FileTemplatePartial
{
    public class FileTemplatePartialRegistrations : FilePerModelTemplateRegistration<FileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public FileTemplatePartialRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => FileTemplatePartialTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, FileTemplateModel model)
        {
            return new FileTemplatePartialTemplate(TemplateId, project, model);
        }

        public override IEnumerable<FileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetFileTemplateModels();
        }
    }
}