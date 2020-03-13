using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiStereotypeExtensionsTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiStereotypeExtensionsTemplateRegistration : ModelTemplateRegistrationBase<IStereotypeDefinition>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiStereotypeExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiStereotypeExtensionsTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IStereotypeDefinition model)
        {
            return new ApiStereotypeExtensionsTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IStereotypeDefinition> GetModels(IApplication application)
        {
            return _metadataManager.GetMetadata<IStereotypeDefinition>("Module Builder", application.Id);
        }
    }
}