using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Dart.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Dart.Templates.DartFileTemplatePartial
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DartFileTemplatePartialTemplateRegistration : FilePerModelTemplateRegistration<DartFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DartFileTemplatePartialTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DartFileTemplatePartialTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, DartFileTemplateModel model)
        {
            return new DartFileTemplatePartialTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<DartFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetDartFileTemplateModels();
        }
    }
}