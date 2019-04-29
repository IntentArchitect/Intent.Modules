using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplate;
using Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePartial;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePreProcessedFile
{
    public class ProjectItemTemplatePreProcessedFileRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public ProjectItemTemplatePreProcessedFileRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => "Intent.ModuleBuilder.ProjectItemTemplate.T4Template.PreProcessed";

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: ProjectItemTemplateTemplate.TemplateId,
                partialTemplateId: ProjectItemTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<IClass> GetModels(IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => x.IsFileTemplate())
                .ToList();
        }
    }
}
