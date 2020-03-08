using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModelerBuilder;
using Intent.Modules.ModuleBuilder.Api.Modeler;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.ModelerConfig
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ModelerConfigRegistration : ModelTemplateRegistrationBase<Modeler>
    {
        private readonly IMetadataManager _metadataManager;

        public ModelerConfigRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ModuleBuilder.Templates.ModelerConfig.ModelerConfig.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, Modeler model)
        {
            return new ModuleBuilder.Templates.ModelerConfig.ModelerConfig(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<Modeler> GetModels(IApplication application)
        {
            var modelers = _metadataManager
                .GetMetadata<IElement>("Module Builder")
                .Where(x => x.Application.Id == application.Id && (x.SpecializationType == Constants.ElementSpecializationTypes.Modeler || x.SpecializationType == Constants.ElementSpecializationTypes.ModelerExtension))
                .Select(x => new Modeler(x))
                .ToList();

            return modelers;
        }
    }
}