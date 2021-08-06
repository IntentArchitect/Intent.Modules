using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.ModuleBuilder.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ModuleSettingsExtensionsTemplateRegistration : SingleFileListModelTemplateRegistration<ModuleSettingsConfigurationModel>
    {
        public override string TemplateId => ModuleSettingsExtensionsTemplate.TemplateId;
        private readonly IMetadataManager _metadataManager;

        public ModuleSettingsExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<ModuleSettingsConfigurationModel> model)
        {
            return new ModuleSettingsExtensionsTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<ModuleSettingsConfigurationModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetModuleSettingsConfigurationModels().ToList();
        }
    }
}