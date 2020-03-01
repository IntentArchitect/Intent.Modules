using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModelerBuilder;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.ModelerConfig
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ModelerConfigRegistration : ModelTemplateRegistrationBase<IElement>
    {
        private readonly IMetadataManager _metadataManager;

        public ModelerConfigRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ModuleBuilder.Templates.ModelerConfig.ModelerConfig.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IElement model)
        {
            return new ModuleBuilder.Templates.ModelerConfig.ModelerConfig(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IElement> GetModels(IApplication application)
        {
            var elements = _metadataManager
                .GetMetadata<IElement>("Module Builder")
                .ToList();

            return elements
                .Where(x => x.SpecializationType == Constants.ElementSpecializationTypes.Modeler)
                .ToList();
        }
    }
}