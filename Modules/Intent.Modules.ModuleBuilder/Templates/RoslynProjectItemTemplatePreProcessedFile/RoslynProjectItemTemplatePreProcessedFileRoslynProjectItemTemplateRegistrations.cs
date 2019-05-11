using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplate;
using Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplatePartial;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplatePreProcessedFile
{
    public class RoslynProjectItemTemplatePreProcessedFileRoslynProjectItemTemplateRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetadataManager _metaDataManager;

        public RoslynProjectItemTemplatePreProcessedFileRoslynProjectItemTemplateRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => "Intent.ModuleBuilder.RoslynProjectItemTemplate.T4Template.PreProcessed";

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: RoslynProjectItemTemplateTemplate.TemplateId,
                partialTemplateId: RoslynProjectItemTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<IClass> GetModels(IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => x.IsCSharpTemplate())
                .ToList();
        }
    }
}
