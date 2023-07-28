using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.TypeScript.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.TypeScript.Templates.TypescriptTemplateStringInterpolation
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class TypescriptTemplateStringInterpolationTemplateRegistration : FilePerModelTemplateRegistration<TypescriptFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public TypescriptTemplateStringInterpolationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TypescriptTemplateStringInterpolationTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, TypescriptFileTemplateModel model)
        {
            return new TypescriptTemplateStringInterpolationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<TypescriptFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetTypescriptFileTemplateModels()
                .Where(x => x.GetTypeScriptTemplateSettings()?.TemplatingMethod().IsStringInterpolation() == true)
                .ToList();
        }
    }
}