using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Modelers.WebClient.Proxies.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.Modelers.ServiceProxies.Api
{
    [IntentManaged(Mode.Merge)]
    public class ServiceProxiesPackageModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Service Proxies Package";
        public const string SpecializationTypeId = "46ef9b5e-e38c-48f2-8516-ea48c310ce23";

        [IntentManaged(Mode.Ignore)]
        public ServiceProxiesPackageModel(IPackage package)
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

        public IList<ServiceProxyModel> ServiceProxies => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == ServiceProxyModel.SpecializationType)
            .Select(x => new ServiceProxyModel(x))
            .ToList();

        public IList<FolderModel> Folders => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == FolderModel.SpecializationType)
            .Select(x => new FolderModel(x))
            .ToList();

    }
}