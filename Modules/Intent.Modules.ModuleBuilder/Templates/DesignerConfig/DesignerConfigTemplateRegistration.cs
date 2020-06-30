using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.DesignerConfig
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DesignerConfigTemplateRegistration : ModelTemplateRegistrationBase<DesignerModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DesignerConfigTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DesignerConfigTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, DesignerModel model)
        {
            return new DesignerConfigTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<DesignerModel> GetModels(IApplication application)
        {
            var modelers = _metadataManager
                .GetDesignerModels(application)
                .ToList();

            return modelers;
        }
    }
}