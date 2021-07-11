using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Kotlin.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Kotlin.Templates.Templates.KotlinFileTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class KotlinFileTemplateRegistration : FilePerModelTemplateRegistration<KotlinFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public KotlinFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => KotlinFileTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, KotlinFileTemplateModel model)
        {
            return new KotlinFileTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<KotlinFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetKotlinFileTemplateModels();
        }
    }
}