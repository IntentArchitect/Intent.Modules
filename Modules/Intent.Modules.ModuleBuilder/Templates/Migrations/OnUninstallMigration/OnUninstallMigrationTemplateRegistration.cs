using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Migrations.OnUninstallMigration
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class OnUninstallMigrationTemplateRegistration : FilePerModelTemplateRegistration<OnUninstallMigrationModel>
    {
        private readonly IMetadataManager _metadataManager;

        public OnUninstallMigrationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => OnUninstallMigrationTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, OnUninstallMigrationModel model)
        {
            return new OnUninstallMigrationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<OnUninstallMigrationModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetOnUninstallMigrationModels();
        }
    }
}