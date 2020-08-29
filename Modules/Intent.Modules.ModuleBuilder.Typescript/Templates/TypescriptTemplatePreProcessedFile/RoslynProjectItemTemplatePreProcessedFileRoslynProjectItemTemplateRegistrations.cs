using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Modules.ModuleBuilder.Typescript.Api;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Typescript.Templates.TypescriptTemplatePreProcessedFile
{
    public class RoslynProjectItemTemplatePreProcessedFileRoslynProjectItemTemplateRegistrations : ModelTemplateRegistrationBase<TypescriptFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public RoslynProjectItemTemplatePreProcessedFileRoslynProjectItemTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => "Intent.ModuleBuilder.RoslynProjectItemTemplate.T4Template.PreProcessed";

        public override ITemplate CreateTemplateInstance(IProject project, TypescriptFileTemplateModel model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: TypescriptTemplate.TypescriptTemplate.TemplateId,
                partialTemplateId: TypescriptTemplatePartial.TypescriptTemplatePartial.TemplateId);
        }

        public override IEnumerable<TypescriptFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.GetTypescriptFileTemplateModels(application);
        }
    }
}
