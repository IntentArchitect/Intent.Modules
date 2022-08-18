using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Java.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Java.Templates.JavaFileStringInterpolation
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class JavaFileStringInterpolationTemplateRegistration : FilePerModelTemplateRegistration<JavaFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public JavaFileStringInterpolationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => JavaFileStringInterpolationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, JavaFileTemplateModel model)
        {
            return new JavaFileStringInterpolationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<JavaFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetJavaFileTemplateModels()
                .Where(x => !x.HasJavaTemplateSettings() || x.GetJavaTemplateSettings().TemplatingMethod().IsStringInterpolation())
                .ToList();
        }
    }
}