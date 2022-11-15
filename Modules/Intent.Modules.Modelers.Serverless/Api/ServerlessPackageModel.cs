using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.Api
{
    [IntentManaged(Mode.Fully)]
    public class ServerlessPackageModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Serverless Package";
        public const string SpecializationTypeId = "317ea0aa-0713-4bc0-bbd1-f2980b75f6a0";

        [IntentManaged(Mode.Ignore)]
        public ServerlessPackageModel(IPackage package)
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

        public IList<PayloadModel> Payloads => UnderlyingPackage.ChildElements
            .GetElementsOfType(PayloadModel.SpecializationTypeId)
            .Select(x => new PayloadModel(x))
            .ToList();

    }
}