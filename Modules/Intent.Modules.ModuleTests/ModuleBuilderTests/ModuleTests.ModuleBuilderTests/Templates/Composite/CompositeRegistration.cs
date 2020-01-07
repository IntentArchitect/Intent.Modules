using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Templates;
using Intent.Modelers.Domain.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Templates.Composite
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CompositeRegistration : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetadataManager _metadataManager;

        public CompositeRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => Composite.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new Composite(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IClass> GetModels(IApplication application)
        {
            // Filter classes by SpecializationType if necessary (e.g. .Where(x => x.SpecializationType == "Service") for services only)
            return _metadataManager.GetDomainClasses(application.Id);
        }
    }
}