using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ApplicationTemplate.Builder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Templates.Templates.InstallationSettingsFile
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class InstallationSettingsFileTemplateRegistration : FilePerModelTemplateRegistration<InstallationSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public InstallationSettingsFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => InstallationSettingsFileTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, InstallationSettingsModel model)
        {
            return new InstallationSettingsFileTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<InstallationSettingsModel> GetModels(IApplication application)
        {
            return _metadataManager.AppTemplates(application).GetInstallationSettingsModels();
        }
    }
}