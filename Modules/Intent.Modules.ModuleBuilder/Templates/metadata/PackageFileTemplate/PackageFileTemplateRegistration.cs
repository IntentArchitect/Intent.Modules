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

namespace Intent.Modules.ModuleBuilder.Templates.metadata.PackageFileTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class PackageFileTemplateRegistration : ModelTemplateRegistrationBase<IModulePackage>
    {
        private readonly IMetadataManager _metadataManager;

        public PackageFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => PackageFileTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IModulePackage model)
        {
            return new PackageFileTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IModulePackage> GetModels(IApplication application)
        {
            return _metadataManager.GetModulePackages(application);
        }
    }
}