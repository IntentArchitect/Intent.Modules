using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.DesignerSettings
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DesignerSettingsTemplateRegistration : FilePerModelTemplateRegistration<DesignerSettingsModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DesignerSettingsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DesignerSettingsTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, DesignerSettingsModel model)
        {
            return new DesignerSettingsTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<DesignerSettingsModel> GetModels(IApplication application)
        {
            var modelers = _metadataManager
                .ModuleBuilder(application).GetDesignerSettingsModels()
                .Where(x => !x.GetDesignerSettings().IsReference())
                .ToList();

            return modelers;
        }
    }
}