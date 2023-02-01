using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Dart.Api;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Dart.Templates.DartFileTemplatePartial;
using Intent.Modules.ModuleBuilder.Dart.Templates.DartFileTemplateT4;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Dart.Templates.DartTemplatePreProcessedFile
{
    public class DartTemplatePreProcessedFileRegistrations : FilePerModelTemplateRegistration<DartFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DartTemplatePreProcessedFileRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => "Intent.ModuleBuilder.Dart.Templates.DartTemplate.PreProcessT4";

        public override ITemplate CreateTemplateInstance(IOutputTarget project, DartFileTemplateModel model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: DartFileTemplateT4Template.TemplateId,
                partialTemplateId: DartFileTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<DartFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetDartFileTemplateModels()
                .Where(x => !x.TryGetDartTemplateSettings(out var templateSettings) || templateSettings.TemplatingMethod().IsT4Template())
                .ToList();
        }
    }
}

