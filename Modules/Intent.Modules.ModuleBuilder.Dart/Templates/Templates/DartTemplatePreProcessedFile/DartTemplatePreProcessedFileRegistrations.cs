//using System.Collections.Generic;
//using System.Linq;
//using Intent.Engine;
//using Intent.ModuleBuilder.Api;
//using Intent.Modules.Common.Registrations;
//using Intent.Modules.ModuleBuilder.Dart.Api;
//using Intent.Modules.ModuleBuilder.Dart.Templates.Templates.DartTemplatePartial;
//using Intent.Modules.ModuleBuilder.Dart.Templates.Templates.DartTemplateT4;
//using Intent.Modules.ModuleBuilder.Templates.Common;
//using Intent.Templates;

//namespace Intent.Modules.ModuleBuilder.Dart.Templates.Templates.DartTemplatePreProcessedFile
//{
//    public class DartTemplatePreProcessedFileRegistrations : FilePerModelTemplateRegistration<DartFileTemplateModel>
//    {
//        private readonly IMetadataManager _metadataManager;

//        public DartTemplatePreProcessedFileRegistrations(IMetadataManager metadataManager)
//        {
//            _metadataManager = metadataManager;
//        }

//        public override string TemplateId => "ModuleBuilder.Dart.Templates.DartTemplate.PreProcessT4";

//        public override ITemplate CreateTemplateInstance(IOutputTarget project, DartFileTemplateModel model)
//        {
//            return new TemplatePreProcessedFileTemplate(
//                templateId: TemplateId,
//                project: project,
//                model: model,
//                t4TemplateId: DartTemplateT4Template.TemplateId,
//                partialTemplateId: DartTemplatePartialTemplate.TemplateId);
//        }

//        public override IEnumerable<DartFileTemplateModel> GetModels(IApplication application)
//        {
//            return _metadataManager.ModuleBuilder(application).GetDartFileTemplateModels()
//                .Where(x => !x.TryGetDartTemplateSettings(out var templateSettings) || templateSettings.TemplatingMethod().IsT4Template())
//                .ToList();
//        }
//    }
//}

