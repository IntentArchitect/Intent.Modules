using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.TypeScript.Api;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Modules.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial;
using Intent.Modules.ModuleBuilder.TypeScript.Templates.TypescriptTemplateT4;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Typescript.Templates.TypescriptTemplatePreProcessedFile
{
    public class TypescriptTemplatePreProcessedFileRegistrations : FilePerModelTemplateRegistration<TypescriptFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public TypescriptTemplatePreProcessedFileRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => "ModuleBuilder.Typescript.Templates.TypescriptTemplate.PreProcessT4";

        public override ITemplate CreateTemplateInstance(IOutputTarget project, TypescriptFileTemplateModel model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: TypescriptTemplateT4Template.TemplateId,
                partialTemplateId: TypescriptTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<TypescriptFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetTypescriptFileTemplateModels()
                .Where(x => !x.TryGetTypeScriptTemplateSettings(out var templateSettings) || templateSettings.TemplatingMethod().IsT4Template())
                .ToList();
        }
    }
}
