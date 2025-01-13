using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Modelers.Types.ServiceProxies
{
    public static class ExtensionMethods
    {
        private static readonly IReadOnlyCollection<string> HttpSettingsStereotypeOption = ["Http Settings"];

        public static string[] GetPackageNamespaceParts(IElement element)
        {
            return element.Package.Name.Split('.');
        }

        public static IList<DTOModel> GetMappedServiceProxyDTOModels(this IDesigner designer)
        {
            return GetMappedServiceProxyDTOModels(designer, HttpSettingsStereotypeOption);
        }

        public static IList<DTOModel> GetMappedServiceProxyDTOModels(this IDesigner designer, string stereotypeName)
        {
            return GetMappedServiceProxyDTOModels(designer, [stereotypeName]);
        }

        public static IList<DTOModel> GetMappedServiceProxyDTOModels(this IDesigner designer, IReadOnlyCollection<string> stereotypeNames)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(proxyModel => GetReferencedDTOModels(proxyModel, stereotypeNames))
                .DistinctBy(x => x.Id)
                .ToList();
        }

        public static IList<DTOModel> GetMappedServiceProxyInboundDTOModels(this IDesigner designer)
        {
            return GetMappedServiceProxyInboundDTOModels(designer, HttpSettingsStereotypeOption);
        }

        public static IList<DTOModel> GetMappedServiceProxyInboundDTOModels(this IDesigner designer, string stereotypeName)
        {
            return GetMappedServiceProxyInboundDTOModels(designer, [stereotypeName]);
        }

        public static IList<DTOModel> GetMappedServiceProxyInboundDTOModels(this IDesigner designer, IReadOnlyCollection<string> stereotypeNames)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(proxyModel => GetReferencedDTOModels(proxyModel, includeReturnTypes: false, stereotypeNames))
                .DistinctBy(x => x.Id)
                .ToList();
        }

        public static IReadOnlyCollection<EnumModel> GetMappedServiceProxyEnumModels(this IDesigner designer)
        {
            return GetMappedServiceProxyEnumModels(designer, HttpSettingsStereotypeOption);
        }

        public static IReadOnlyCollection<EnumModel> GetMappedServiceProxyEnumModels(this IDesigner designer, string stereotypeName)
        {
            return GetMappedServiceProxyEnumModels(designer, [stereotypeName]);
        }

        public static IReadOnlyCollection<EnumModel> GetMappedServiceProxyEnumModels(this IDesigner designer, IReadOnlyCollection<string> stereotypeNames)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(proxyModel => GetReferencedEnumModels(proxyModel, stereotypeNames))
                .DistinctBy(x => x.Id)
                .ToList();
        }

        internal static IEnumerable<DTOModel> GetReferencedDTOModels(this ServiceProxyModel proxy, IReadOnlyCollection<string> stereotypeNames)
        {
            return GetReferencedDTOModels(proxy, includeReturnTypes: true, stereotypeNames);
        }

        internal static IEnumerable<DTOModel> GetReferencedDTOModels(this ServiceProxyModel proxy, bool includeReturnTypes, IReadOnlyCollection<string> stereotypeNames)
        {
            return DeepGetDistinctReferencedElements(GetMappedEndpoints(proxy, stereotypeNames), includeReturnTypes)
                .Where(x => x.SpecializationTypeId is not (TypeDefinitionModel.SpecializationTypeId or EnumModel.SpecializationTypeId))
                .Select(x => new DTOModel(x))
                .ToList();
        }

        internal static IEnumerable<EnumModel> GetReferencedEnumModels(this ServiceProxyModel proxy, IReadOnlyCollection<string> stereotypeNames)
        {
            return DeepGetDistinctReferencedElements(GetMappedEndpoints(proxy, stereotypeNames))
                .Where(x => x.SpecializationTypeId is EnumModel.SpecializationTypeId)
                .Select(x => new EnumModel(x))
                .ToList();
        }

        private static HashSet<IElement> DeepGetDistinctReferencedElements(
            IEnumerable<IElement> elements,
            bool includeReturnTypes = true)
        {
            var referencedElements = new HashSet<IElement>(ElementComparer.Instance);
            var workingStack = new Stack<IElement>(elements);

            while (workingStack.Any())
            {
                var currentElement = workingStack.Pop();
                var isDataContract = currentElement.SpecializationType is "DTO" or "Command" or "Query";

                if ((includeReturnTypes || !isDataContract) &&
                    currentElement.TypeReference?.Element is IElement referencedElement &&
                    referencedElements.Add(referencedElement)) // Avoid infinite loops due to cyclic references
                {
                    foreach (var childElement in referencedElement.ChildElements)
                    {
                        workingStack.Push(childElement);
                    }
                }

                foreach (var genericArgument in currentElement.TypeReference?.GenericTypeParameters ?? new List<ITypeReference>())
                {
                    if (genericArgument?.Element is not IElement genericArgumentType ||
                        !referencedElements.Add(genericArgumentType))
                    {
                        continue; // Avoid infinite loops due to cyclic references
                    }

                    foreach (var childElement in genericArgumentType.ChildElements)
                    {
                        workingStack.Push(childElement);
                    }
                }


                if (isDataContract)
                {
                    referencedElements.Add(currentElement);
                }

                foreach (var childElement in currentElement.ChildElements)
                {
                    workingStack.Push(childElement);
                }
            }

            return referencedElements;
        }

        private static IEnumerable<IElement> GetMappedEndpoints(ServiceProxyModel proxyModel, IReadOnlyCollection<string> stereotypeNames)
        {
            var stereotypeSet = new HashSet<string>(stereotypeNames);

            if (proxyModel.Mapping?.Element?.IsServiceModel() == true)
            {
                IEnumerable<Intent.Modelers.Services.Api.OperationModel> mappedOperations = proxyModel.MappedService.Operations;
                // Filter when proxy operations are present.
                // Backwards compatibility - when we didn't have operations on service proxies.
                if (proxyModel.Operations.Any())
                {
                    mappedOperations = mappedOperations
                        .Where(mappedOperation => proxyModel.Operations
                            .Any(proxyOperation => proxyOperation.Mapping?.ElementId == mappedOperation.Id));
                }

                return mappedOperations
                    .Select(x => x.InternalElement)
                    .Where(x => x.Stereotypes.Any(s => stereotypeSet.Contains(s.Name)));
            }

            return proxyModel.Operations
                .Select(x => x.Mapping?.Element)
                .Cast<IElement>()
                .Where(x => x.Stereotypes.Any(s => stereotypeSet.Contains(s.Name)));
        }

        private class ElementComparer : IEqualityComparer<IElement>
        {
            private ElementComparer() { }

            public static ElementComparer Instance { get; } = new ElementComparer();

            public bool Equals(IElement x, IElement y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null) return false;
                if (y is null) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(IElement obj)
            {
                return (obj.Id != null ? obj.Id.GetHashCode() : 0);
            }
        }
    }
}