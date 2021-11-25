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

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CSharpTemplatePartialTemplateRegistration : FilePerModelTemplateRegistration<CSharpTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public CSharpTemplatePartialTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CSharpTemplatePartialTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, CSharpTemplateModel model)
        {
            return new CSharpTemplatePartialTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<CSharpTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetCSharpTemplateModels();
        }
    }
}