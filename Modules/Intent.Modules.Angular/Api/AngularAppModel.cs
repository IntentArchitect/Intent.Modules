using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.Angular.Api
{
    [IntentManaged(Mode.Merge)]
    public class AngularAppModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Angular App";
        public const string SpecializationTypeId = "fc44141e-8079-4f63-894b-f1a26736c991";

        [IntentManaged(Mode.Ignore)]
        public AngularAppModel(IPackage package)
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
    }
}