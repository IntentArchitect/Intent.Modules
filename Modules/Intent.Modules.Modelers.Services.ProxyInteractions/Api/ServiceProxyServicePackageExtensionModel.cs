using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.Services.ProxyInteractions.Api
{
    [IntentManaged(Mode.Merge)]
    public class ServiceProxyServicePackageExtensionModel : ServicesPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public ServiceProxyServicePackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<ServiceProxyModel> ServiceProxies => UnderlyingPackage.ChildElements
            .GetElementsOfType(ServiceProxyModel.SpecializationTypeId)
            .Select(x => new ServiceProxyModel(x))
            .ToList();

    }
}