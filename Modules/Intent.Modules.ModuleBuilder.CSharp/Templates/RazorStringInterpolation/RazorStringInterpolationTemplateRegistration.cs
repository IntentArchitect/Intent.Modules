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

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.RazorStringInterpolation
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class RazorStringInterpolationTemplateRegistration : FilePerModelTemplateRegistration<RazorTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public RazorStringInterpolationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RazorStringInterpolationTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, RazorTemplateModel model)
        {
            return new RazorStringInterpolationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<RazorTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetRazorTemplateModels()
                .Where(x => x.GetRazorTemplateSettings()?.TemplatingMethod().IsStringInterpolation() == true)
                .ToList();
        }
    }
}