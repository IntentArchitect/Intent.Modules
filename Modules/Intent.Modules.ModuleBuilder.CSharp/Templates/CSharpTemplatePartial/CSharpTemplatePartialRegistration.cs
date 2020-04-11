using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.CSharp.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CSharpTemplatePartialRegistration : ModelTemplateRegistrationBase<CSharpTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public CSharpTemplatePartialRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CSharpTemplatePartial.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, CSharpTemplateModel model)
        {
            return new CSharpTemplatePartial(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<CSharpTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.GetCSharpTemplateModels(application);
        }
    }
}