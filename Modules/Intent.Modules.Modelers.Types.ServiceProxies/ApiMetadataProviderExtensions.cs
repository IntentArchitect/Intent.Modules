using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Modelers.Types.ServiceProxies;
using Intent.RoslynWeaver.Attributes;
using EnumModel = Intent.Modules.Common.Types.Api.EnumModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.Types.ServiceProxies.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ServiceProxyModel> GetServiceProxyModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ServiceProxyModel.SpecializationTypeId)
                .Select(x => new ServiceProxyModel(x))
                .ToList();
        }
    }
}