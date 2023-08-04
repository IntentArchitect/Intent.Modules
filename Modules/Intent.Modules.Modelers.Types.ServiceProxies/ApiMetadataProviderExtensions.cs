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

        [Obsolete]
        [IntentManaged(Mode.Ignore)]
        public static IList<ServiceProxyEnumModel> GetServiceProxyEnumModels(this IDesigner designer)
        {
            throw new InvalidOperationException("Unsupported");
        }

        /// <summary>
        /// Obsolete. Use <see cref="ExtensionMethods.GetMappedServiceProxyDTOModels"/> instead.
        /// </summary>
        [Obsolete]
        [IntentManaged(Mode.Ignore)]
        public static IList<ServiceProxyDTOModel> GetServiceProxyDTOModels(this IDesigner designer)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(s => s.GetDTOModels())
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Obsolete. Use <see cref="ExtensionMethods.GetMappedServiceProxyDTOModels"/> instead.
        /// </summary>
        [Obsolete]
        [IntentManaged(Mode.Ignore)]
        public static IList<ServiceProxyDTOModel> GetProxyMappedServiceDTOModels(this IDesigner designer)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(s => s.GetMappedServiceDTOModels())
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Obsolete. Use <see cref="ExtensionMethods.GetMappedServiceProxyEnumModels"/> instead.
        /// </summary>
        [Obsolete]
        [IntentManaged(Mode.Ignore)]
        public static IReadOnlyCollection<ServiceProxyEnumModel> GetProxyMappedEnumModels(this IDesigner designer)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(s => s.GetMappedEnumModels())
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Obsolete. Use <see cref="ExtensionMethods.GetReferencedDTOModels"/> instead.
        /// </summary>
        [Obsolete]
        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<ServiceProxyDTOModel> GetDTOModels(this ServiceProxyModel proxy)
        {
            return proxy.GetReferencedDTOModels()
                .Select(x => new ServiceProxyDTOModel(x.InternalElement, proxy))
                .ToList();
        }

        /// <summary>
        /// Obsolete. Use <see cref="ExtensionMethods.GetMappedServiceProxyDTOModels"/> instead.
        /// </summary>
        [Obsolete]
        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<ServiceProxyDTOModel> GetMappedServiceDTOModels(this ServiceProxyModel proxy)
        {
            return proxy.GetReferencedDTOModels()
                .Select(x => new ServiceProxyDTOModel(x.InternalElement, proxy))
                .ToList();
        }

        /// <summary>
        /// Obsolete. Use <see cref="ExtensionMethods.GetMappedServiceProxyEnumModels"/> instead.
        /// </summary>
        [Obsolete]
        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<ServiceProxyEnumModel> GetMappedEnumModels(this ServiceProxyModel proxy)
        {
            return proxy.GetReferencedEnumModels()
                .Select(x => new ServiceProxyEnumModel(x.InternalElement, proxy))
                .ToList();
        }
    }
}