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
        public static string[] GetPackageNamespaceParts(IElement element)
        {
            return element.Package.Name.Split('.');
        }

        public static IList<DTOModel> GetMappedServiceProxyDTOModels(this IDesigner designer)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(GetReferencedDTOModels)
                .Distinct()
                .ToList();
        }

        public static IList<DTOModel> GetMappedServiceProxyInboundDTOModels(this IDesigner designer)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(model => GetReferencedDTOModels(model, includeReturnTypes: false))
                .Distinct()
                .ToList();
        }

        public static IReadOnlyCollection<EnumModel> GetMappedServiceProxyEnumModels(this IDesigner designer)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(GetReferencedEnumModels)
                .Distinct()
                .ToList();
        }

        internal static IEnumerable<DTOModel> GetReferencedDTOModels(this ServiceProxyModel proxy)
        {
            return GetReferencedDTOModels(proxy, includeReturnTypes: true);
        }

        internal static IEnumerable<DTOModel> GetReferencedDTOModels(this ServiceProxyModel proxy, bool includeReturnTypes)
        {
            return DeepGetDistinctReferencedElements(GetMappedEndpoints(proxy), includeReturnTypes)
                .Where(x => x.SpecializationTypeId is not (TypeDefinitionModel.SpecializationTypeId or EnumModel.SpecializationTypeId))
                .Select(x => new DTOModel(x))
                .ToList();
        }

        internal static IEnumerable<EnumModel> GetReferencedEnumModels(this ServiceProxyModel proxy)
        {
            return DeepGetDistinctReferencedElements(GetMappedEndpoints(proxy))
                .Where(x => x.SpecializationTypeId is EnumModel.SpecializationTypeId)
                .Select(x => new EnumModel(x))
                .ToList();
        }

        private static ISet<IElement> DeepGetDistinctReferencedElements(
            IEnumerable<IElement> elements,
            bool includeReturnTypes = true)
        {
            var referencedElements = new HashSet<IElement>();
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

        private static IEnumerable<IElement> GetMappedEndpoints(ServiceProxyModel proxyModel)
        {
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
                    .Where(x => x.Stereotypes.Any(s => s.Name == "Http Settings"));
            }

            return proxyModel.Operations
                .Select(x => x.Mapping?.Element)
                .Cast<IElement>()
                .Where(x => x.Stereotypes.Any(s => s.Name == "Http Settings"));
        }
    }
}
