using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModelExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModelExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiElementModelExtensionsTemplateRegistration : FilePerModelTemplateRegistration<ExtensionModel>
    {
        private readonly IMetadataManager _metadataManager;
        private IEnumerable<IStereotypeDefinition> _stereotypeDefinitions;

        public ApiElementModelExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiElementModelExtensionsTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, ExtensionModel model)
        {
            return new ApiElementModelExtensionsTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ExtensionModel> GetModels(IApplication application)
        {
            _stereotypeDefinitions = _metadataManager.ModuleBuilder(application).StereotypeDefinitions;
            var targetTypes = _stereotypeDefinitions.SelectMany(x => x.TargetElements).Distinct();
            return targetTypes.Select(x => new ExtensionModel(new ExtensionModelType(x), _stereotypeDefinitions.Where(s => s.TargetElements.Any(t => t.Id.Equals(x.Id, StringComparison.InvariantCultureIgnoreCase))).ToList()));
        }
    }
}