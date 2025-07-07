#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Modelers.Types.ServiceProxies
{
    public static class ExtensionMethods
    {
        private static readonly IReadOnlyList<string> HttpSettingsStereotypeOption = ["Http Settings"];

        public static string[] GetPackageNamespaceParts(IElement element)
        {
            return element.Package.Name.Split('.');
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetServiceProxyReferencedDtos"/> instead.
        /// </summary>
        [Obsolete("See XML doc comments")]
        public static IList<DTOModel> GetMappedServiceProxyDTOModels(this IDesigner designer)
        {
            return GetMappedServiceProxyDTOModels(designer, HttpSettingsStereotypeOption);
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetServiceProxyReferencedDtos"/> instead.
        /// </summary>
        [Obsolete("See XML doc comments")]
        public static IList<DTOModel> GetMappedServiceProxyDTOModels(this IDesigner designer, string stereotypeName)
        {
            return GetMappedServiceProxyDTOModels(designer, [stereotypeName]);
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetServiceProxyReferencedDtos"/> instead.
        /// </summary>
        [Obsolete("See XML doc comments")]
        public static IList<DTOModel> GetMappedServiceProxyDTOModels(this IDesigner designer, IReadOnlyCollection<string> stereotypeNames)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(proxy => GetReferencedDtoModels(GetMappedEndpointElements(proxy, stereotypeNames)))
                .ToList();
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetServiceProxyReferencedDtos"/> instead.
        /// </summary>
        [Obsolete("See XML doc comments")]
        public static IList<DTOModel> GetMappedServiceProxyInboundDTOModels(this IDesigner designer)
        {
            return GetMappedServiceProxyInboundDTOModels(designer, HttpSettingsStereotypeOption);
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetServiceProxyReferencedDtos"/> instead.
        /// </summary>
        [Obsolete("See XML doc comments")]
        public static IList<DTOModel> GetMappedServiceProxyInboundDTOModels(this IDesigner designer, string stereotypeName)
        {
            return GetMappedServiceProxyInboundDTOModels(designer, [stereotypeName]);
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetServiceProxyReferencedDtos"/> instead.
        /// </summary>
        [Obsolete("See XML doc comments")]
        public static IList<DTOModel> GetMappedServiceProxyInboundDTOModels(this IDesigner designer, IReadOnlyCollection<string> stereotypeNames)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(proxy => GetReferencedDtoModels(GetMappedEndpointElements(proxy, stereotypeNames), includeReturnTypes: false))
                .ToList();
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetServiceProxyReferencedEnums"/> instead.
        /// </summary>
        [Obsolete("See XML doc comments")]
        public static IReadOnlyCollection<EnumModel> GetMappedServiceProxyEnumModels(this IDesigner designer)
        {
            return GetMappedServiceProxyEnumModels(designer, HttpSettingsStereotypeOption);
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetServiceProxyReferencedEnums"/> instead.
        /// </summary>
        [Obsolete("See XML doc comments")]
        public static IReadOnlyCollection<EnumModel> GetMappedServiceProxyEnumModels(this IDesigner designer, string stereotypeName)
        {
            return GetMappedServiceProxyEnumModels(designer, [stereotypeName]);
        }

        /// <summary>
        /// Obsolete. Use <see cref="GetServiceProxyReferencedEnums"/> instead.
        /// </summary>
        [Obsolete("See XML doc comments")]
        public static IReadOnlyCollection<EnumModel> GetMappedServiceProxyEnumModels(this IDesigner designer, IReadOnlyCollection<string> stereotypeNames)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(proxyModel => GetReferencedEnumModels(GetMappedEndpointElements(proxyModel, stereotypeNames)))
                .ToList();
        }

        public static IReadOnlyList<DTOModel> GetServiceProxyReferencedDtos(
            this IMetadataManager metadataManager,
            string applicationId,
            bool includeReturnTypes,
            IReadOnlyList<string>? stereotypeNames,
            IReadOnlyCollection<Func<string, IDesigner>> getDesigners)
        {
            stereotypeNames ??= HttpSettingsStereotypeOption;
            var designers = getDesigners
                .Select(getDesigner => getDesigner(applicationId))
                .ToArray();

            var endpointElements = designers
                .SelectMany(x => x.GetServiceProxyModels())
                .SelectMany(x => GetMappedEndpointElements(x, stereotypeNames))
                .Concat(GetInvokedEndpointElements(designers, stereotypeNames));

            return GetReferencedDtoModels(endpointElements, includeReturnTypes).ToList();
        }

        public static IReadOnlyList<EnumModel> GetServiceProxyReferencedEnums(
            this IMetadataManager metadataManager,
            string applicationId,
            IReadOnlyList<string>? stereotypeNames,
            IReadOnlyCollection<Func<string, IDesigner>> getDesigners)
        {
            stereotypeNames ??= HttpSettingsStereotypeOption;
            var designers = getDesigners
                .Select(getDesigner => getDesigner(applicationId))
                .ToArray();

            var endpointElements = designers
                .SelectMany(x => x.GetServiceProxyModels())
                .SelectMany(x => GetMappedEndpointElements(x, stereotypeNames))
                .Concat(GetInvokedEndpointElements(designers, stereotypeNames));

            return GetReferencedEnumModels(endpointElements).ToList();
        }

        private static IEnumerable<IElement> GetInvokedEndpointElements(
            IReadOnlyList<IDesigner> designers,
            IReadOnlyList<string> stereotypeNames)
        {
            const string performInvocationTypeId = "3e69085c-fa2f-44bd-93eb-41075fd472f8";
            const string callServiceOperationActionTypeId = "fe5a5cd8-aabd-472f-8d42-f5c233e658dc";

            var localPackageIds = designers
                .SelectMany(x => x.Packages)
                .Select(x => x.Id)
                .ToHashSet();

            return Enumerable.Empty<IAssociation>()
                .Concat(designers.SelectMany(x => x.GetAssociationsOfType(performInvocationTypeId)))
                .Concat(designers.SelectMany(x => x.GetAssociationsOfType(callServiceOperationActionTypeId)))
                .Where(x =>
                {
                    var targetElement = x.TargetEnd.TypeReference?.Element as IElement;

                    return targetElement?.Package.Id != null &&
                           !localPackageIds.Contains(targetElement.Package.Id) &&
                           targetElement.Stereotypes.Any(y =>
                               stereotypeNames.Contains(y.Name) || stereotypeNames.Contains(y.DefinitionId));
                })
                .Select(x => (IElement)x.TargetEnd.TypeReference.Element)
                .DistinctBy(x => x.Id);
        }

        private static List<DTOModel> GetReferencedDtoModels(this IEnumerable<IElement> elements, bool includeReturnTypes = true)
        {
            return DeepGetDistinctReferencedElements(elements, includeReturnTypes)
                .Where(x => x.SpecializationTypeId is not (TypeDefinitionModel.SpecializationTypeId or EnumModel.SpecializationTypeId))
                .Select(x => new DTOModel(x is IClosedGenericElement closedGenericElement ? closedGenericElement.OpenGenericElement : x))
                .ToList();
        }

        private static List<EnumModel> GetReferencedEnumModels(this IEnumerable<IElement> elements)
        {
            return DeepGetDistinctReferencedElements(elements)
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

        private static IEnumerable<IElement> GetMappedEndpointElements(ServiceProxyModel proxyModel, IReadOnlyCollection<string> stereotypeNames)
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