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

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CSharpTemplateRegistration : ModelTemplateRegistrationBase<ICSharpTemplate>
    {
        private readonly IMetadataManager _metadataManager;

        public CSharpTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CSharpTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, ICSharpTemplate model)
        {
            return new CSharpTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ICSharpTemplate> GetModels(IApplication application)
        {
            throw new NotImplementedException();
        }
    }
}