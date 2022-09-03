using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Modules.ModuleBuilder.Templates.FileTemplatePartial;
using Intent.Modules.ModuleBuilder.Templates.FileTemplateT4;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Templates.FileTemplatePreProcessedFile
{
    public class FileTemplatePreProcessedFileRegistrations : FilePerModelTemplateRegistration<FileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public FileTemplatePreProcessedFileRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => "Intent.ModuleBuilder.ProjectItemTemplate.T4Template.PreProcessed";

        public override ITemplate CreateTemplateInstance(IOutputTarget project, FileTemplateModel model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: FileTemplateT4Template.TemplateId,
                partialTemplateId: FileTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<FileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetFileTemplateModels()
                .Where(x => x.GetFileSettings().TemplatingMethod().IsT4Template())
                .ToList();
        }
    }
}
