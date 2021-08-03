using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModelPartial
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApiElementModelPartialTemplateRegistration : FilePerModelTemplateRegistration<ElementSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ApiElementModelPartialTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ApiElementModelPartialTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ElementSettingsModel model)
        {
            return new ApiElementModelPartialTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ElementSettingsModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetElementSettingsModels();
        }
    }

    public class ModuleBuilderSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ModuleBuilderSettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public bool CreatePartialAPIModels => bool.TryParse(_groupSettings.GetSetting("b06c3926-23e5-49dd-a59d-93ef16b9777e")?.Value.ToPascalCase(), out var result) && result;
    }

    public static class ModuleSettingsExtensions
    {
        public static ModuleBuilderSettings GetModuleBuilderSettings(this IApplicationSettingsProvider settings)
        {
            return new ModuleBuilderSettings(settings.GetGroup("b2c4252b-cfae-43c5-9682-803aa0b84c87"));
        }
    }
}