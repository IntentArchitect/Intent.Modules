using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.ModuleBuilder.CSharp.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CSharpTemplateRegistration : FilePerModelTemplateRegistration<CSharpTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public CSharpTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CSharpTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, CSharpTemplateModel model)
        {
            return new CSharpTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<CSharpTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetCSharpTemplateModels();
        }
    }
}