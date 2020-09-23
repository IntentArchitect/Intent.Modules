using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge)]
    public class IntentDesignerPackageModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Intent Designer Package";
        public const string SpecializationTypeId = "4e1f79b4-3272-44cb-bee4-7dbd62b16e0e";

        [IntentManaged(Mode.Ignore)]
        public IntentDesignerPackageModel(IPackage package)
        {
            if (!SpecializationType.Equals(package.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from package with specialization type '{package.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            UnderlyingPackage = package;
        }

        public IPackage UnderlyingPackage { get; }
        public string Id => UnderlyingPackage.Id;
        public string Name => UnderlyingPackage.Name;
        public IEnumerable<IStereotype> Stereotypes => UnderlyingPackage.Stereotypes;
        public string FileLocation => UnderlyingPackage.FileLocation;

        [IntentManaged(Mode.Ignore)]
        public string ApiNamespace => this.GetModuleSettings().APINamespace();

        [IntentManaged(Mode.Ignore)]
        public string NuGetPackageId => this.GetModuleSettings().NuGetPackageId();

        [IntentManaged(Mode.Ignore)]
        public string NuGetPackageVersion => this.GetModuleSettings().NuGetPackageVersion();

        [IntentManaged(Mode.Ignore)]
        public string Version => this.GetModuleSettings().Version();
    }
}