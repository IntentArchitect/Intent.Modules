using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.ModuleBuilder.TypeScript.Api;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePreProcessedFile
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
                t4TemplateId: TypescriptTemplate.TypescriptTemplate.TemplateId,
                partialTemplateId: TypescriptTemplatePartial.TypescriptTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<TypescriptFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetTypescriptFileTemplateModels();
        }
    }
}
