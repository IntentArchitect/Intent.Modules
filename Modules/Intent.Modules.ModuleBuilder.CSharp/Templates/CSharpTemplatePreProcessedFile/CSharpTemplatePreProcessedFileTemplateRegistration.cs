using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.CSharp.Api;
using Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpT4;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpTemplatePreProcessedFile
{
    public class CSharpTemplatePreProcessedFileTemplateRegistration : FilePerModelTemplateRegistration<CSharpTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public CSharpTemplatePreProcessedFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => "Intent.ModuleBuilder.CSharp.Templates.CSharpTemplate.T4Template.PreProcessed";

        public override ITemplate CreateTemplateInstance(IOutputTarget project, CSharpTemplateModel model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: CSharpT4Template.TemplateId,
                partialTemplateId: CSharpTemplatePartial.CSharpTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<CSharpTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetCSharpTemplateModels()
                .Where(x => !x.HasCSharpTemplateSettings() || x.GetCSharpTemplateSettings().TemplatingMethod().IsT4Template())
                .ToList();
        }
    }
}
