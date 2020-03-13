using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiModelExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiModelExtensionsRegistration : ModelTemplateRegistrationBase<IElementSettings>
    {
        private readonly IMetadataManager _metadataManager;
        private IEnumerable<IStereotypeDefinition> _stereotypeDefinitions;

        public ApiModelExtensionsRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiModelExtensions.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IElementSettings model)
        {
            return new ApiModelExtensions(project, model, _stereotypeDefinitions.Where(x => x.TargetElements.Any(t => t.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))).ToList());
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IElementSettings> GetModels(IApplication application)
        {
            _stereotypeDefinitions = _metadataManager.GetMetadata<IStereotypeDefinition>("Module Builder", application.Id);
            return _metadataManager.GetElementSettingses(application);
        }
    }
}