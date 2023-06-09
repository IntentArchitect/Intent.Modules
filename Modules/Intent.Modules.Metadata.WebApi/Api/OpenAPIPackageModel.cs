using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    [IntentManaged(Mode.Fully)]
    public class OpenAPIPackageModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Open API Package";
        public const string SpecializationTypeId = "5b78e00d-e532-4c04-9c51-93da258db648";

        [IntentManaged(Mode.Ignore)]
        public OpenAPIPackageModel(IPackage package)
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

        public IList<VersionDefinitionModel> VersionDefinitions => UnderlyingPackage.ChildElements
    .GetElementsOfType(VersionDefinitionModel.SpecializationTypeId)
    .Select(x => new VersionDefinitionModel(x))
    .ToList();

    }
}