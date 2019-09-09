using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplate;
using Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePartial;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePreProcessedFile
{
    public class ProjectItemTemplatePreProcessedFileRegistrations : ModelTemplateRegistrationBase<IFileTemplate>
    {
        private readonly IMetadataManager _metadataManager;

        public ProjectItemTemplatePreProcessedFileRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => "Intent.ModuleBuilder.ProjectItemTemplate.T4Template.PreProcessed";

        public override ITemplate CreateTemplateInstance(IProject project, IFileTemplate model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: ProjectItemTemplateTemplate.TemplateId,
                partialTemplateId: ProjectItemTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<IFileTemplate> GetModels(IApplication applicationManager)
        {
            return _metadataManager.GetFileTemplates(applicationManager)
                .ToList();
        }
    }
}
