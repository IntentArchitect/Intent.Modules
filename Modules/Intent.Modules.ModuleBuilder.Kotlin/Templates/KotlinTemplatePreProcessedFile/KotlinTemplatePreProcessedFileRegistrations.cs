using System.Collections.Generic;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Kotlin.Api;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Kotlin.Templates.KotlinFile;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Kotlin.Templates.Templates.KotlinTemplatePreProcessedFile
{
    public class KotlinTemplatePreProcessedFileRegistrations : FilePerModelTemplateRegistration<KotlinFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public override string TemplateId => "Intent.ModuleBuilder.Kotlin.Templates.KotlinTemplate.PreProcessT4";

        public KotlinTemplatePreProcessedFileRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override ITemplate CreateTemplateInstance(IOutputTarget project, KotlinFileTemplateModel model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: KotlinFileTemplate.TemplateId,
                partialTemplateId: KotlinFileTemplatePartial.KotlinFileTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<KotlinFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetKotlinFileTemplateModels();
        }
    }
}
