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
    public class ApiModelExtensionsRegistration : ModelTemplateRegistrationBase<ExtensionModel>
    {
        private readonly IMetadataManager _metadataManager;
        private IEnumerable<IStereotypeDefinition> _stereotypeDefinitions;

        public ApiModelExtensionsRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiModelExtensions.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, ExtensionModel model)
        {
            return new ApiModelExtensions(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ExtensionModel> GetModels(IApplication application)
        {

            _stereotypeDefinitions = _metadataManager.GetMetadata<IStereotypeDefinition>("Module Builder", application.Id)
                .Where(x => x.TargetMode == StereotypeTargetMode.ElementsOfType);
            var targetTypes = _stereotypeDefinitions.SelectMany(x => x.TargetElements).Where(x => !x.Equals("Package", StringComparison.InvariantCultureIgnoreCase)).Distinct();
            return targetTypes.Select(x => new ExtensionModel(new ExtensionModelType(x), _stereotypeDefinitions.Where(s => s.TargetElements.Any(t => t.Equals(x, StringComparison.InvariantCultureIgnoreCase))).ToList()));
            //return _metadataManager.GetElementSettingsModels(application)
            //    .Where(e => _stereotypeDefinitions.Any(x => x.TargetElements.Any(t => t.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase))))
            //    .Select(model => new ExtensionModel(
            //        element: model, 
            //        stereotypeDefinitions: _stereotypeDefinitions.Where(s => s.TargetElements.Any(t => t.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))).ToList()))
            //    .ToList();
        }
    }
}