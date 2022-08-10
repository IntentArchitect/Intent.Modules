using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.ModuleBuilder.Java.Api;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Java.Templates.JavaTemplatePreProcessedFile
{
    public class JavaTemplatePreProcessedFileRegistrations : FilePerModelTemplateRegistration<JavaFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public override string TemplateId => "Intent.ModuleBuilder.Java.Templates.JavaTemplate.PreProcessT4";

        public JavaTemplatePreProcessedFileRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override ITemplate CreateTemplateInstance(IOutputTarget project, JavaFileTemplateModel model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: JavaFileT4Template.JavaFileT4Template.TemplateId,
                partialTemplateId: JavaFileTemplatePartial.JavaFileTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<JavaFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetJavaFileTemplateModels()
                .Where(x => !x.HasJavaTemplateSettings() || x.GetJavaTemplateSettings().TemplatingMethod().IsT4Template())
                .ToList();
        }
    }
}
