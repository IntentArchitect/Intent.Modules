using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modules.Modelers.UI.ServiceProxies.Api
{
    [IntentManaged(Mode.Merge)]
    public class UserInterfacePackageExtensionModel : UserInterfacePackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public UserInterfacePackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<ServiceProxyModel> ServiceProxies => UnderlyingPackage.ChildElements
            .GetElementsOfType(ServiceProxyModel.SpecializationTypeId)
            .Select(x => new ServiceProxyModel(x))
            .ToList();

    }
}