using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Typescript.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Typescript.Templates.TypescriptTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class TypescriptTemplateRegistration : ModelTemplateRegistrationBase<TypescriptFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public TypescriptTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TypescriptTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, TypescriptFileTemplateModel model)
        {
            return new TypescriptTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<TypescriptFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.GetTypescriptFileTemplateModels(application);
        }
    }
}