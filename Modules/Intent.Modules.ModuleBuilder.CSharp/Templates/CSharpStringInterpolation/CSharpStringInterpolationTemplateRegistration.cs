using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.CSharp.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpStringInterpolation
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CSharpStringInterpolationTemplateRegistration : FilePerModelTemplateRegistration<CSharpTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public CSharpStringInterpolationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CSharpStringInterpolationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, CSharpTemplateModel model)
        {
            return new CSharpStringInterpolationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<CSharpTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetCSharpTemplateModels()
                .Where(x => !x.HasCSharpTemplateSettings() || x.GetCSharpTemplateSettings().TemplatingMethod().IsStringInterpolation())
                .ToList();
        }
    }
}