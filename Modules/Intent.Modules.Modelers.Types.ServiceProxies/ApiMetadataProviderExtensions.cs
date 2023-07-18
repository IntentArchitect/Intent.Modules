using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;
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

        [IntentManaged(Mode.Ignore)]
        public static IList<ServiceProxyEnumModel> GetServiceProxyEnumModels(this IDesigner designer)
        {
            throw new InvalidOperationException("Unsupported");
        }

        [IntentManaged(Mode.Ignore)]
        public static IList<ServiceProxyDTOModel> GetServiceProxyDTOModels(this IDesigner designer)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(s => s.GetDTOModels())
                .Distinct()
                .ToList();
        }

        [IntentManaged(Mode.Ignore)]
        public static IList<ServiceProxyDTOModel> GetProxyMappedServiceDTOModels(this IDesigner designer)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(s => s.GetMappedServiceDTOModels())
                .Distinct()
                .ToList();
        }

        [IntentManaged(Mode.Ignore)]
        public static IReadOnlyCollection<ServiceProxyEnumModel> GetProxyMappedEnumModels(this IDesigner designer)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(s => s.GetMappedEnumModels())
                .Distinct()
                .ToList();
        }

        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<ServiceProxyDTOModel> GetDTOModels(this ServiceProxyModel proxy)
        {
            return DeepGetDistinctReferencedElements(GetMappedEndpoints(proxy))
                .Where(x => x.SpecializationTypeId is not (TypeDefinitionModel.SpecializationTypeId or EnumModel.SpecializationTypeId))
                .Select(x => new ServiceProxyDTOModel(x, proxy))
                .ToList();
        }

        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<ServiceProxyDTOModel> GetMappedServiceDTOModels(this ServiceProxyModel proxy)
        {
            return DeepGetDistinctReferencedElements(GetMappedEndpoints(proxy))
                .Where(x => x.SpecializationTypeId is not (TypeDefinitionModel.SpecializationTypeId or EnumModel.SpecializationTypeId))
                .Select(x => new ServiceProxyDTOModel(x, proxy))
                .ToList();
        }

        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<ServiceProxyEnumModel> GetMappedEnumModels(this ServiceProxyModel proxy)
        {
            return DeepGetDistinctReferencedElements(GetMappedEndpoints(proxy))
                .Where(x => x.SpecializationTypeId is EnumModel.SpecializationTypeId)
                .Select(x => new ServiceProxyEnumModel(x, proxy))
                .ToList();
        }

        [IntentManaged(Mode.Ignore)]
        private static ISet<IElement> DeepGetDistinctReferencedElements(IEnumerable<IElement> elements)
        {
            var referencedElements = new HashSet<IElement>();
            var workingStack = new Stack<IElement>(elements);

            while (workingStack.Any())
            {
                var currentElement = workingStack.Pop();
                if (currentElement.TypeReference?.Element is IElement referencedElement &&
                    referencedElements.Add(referencedElement)) // Avoid infinite loops due to cyclic references
                {
                    foreach (var childElement in referencedElement.ChildElements)
                    {
                        workingStack.Push(childElement);
                    }
                }

                foreach (var genericArgument in currentElement.TypeReference?.GenericTypeParameters ?? new List<ITypeReference>())
                {
                    if (genericArgument?.Element is IElement genericArgumentType &&
                        referencedElements.Add(genericArgumentType)) // Avoid infinite loops due to cyclic references
                    {
                        foreach (var childElement in genericArgumentType.ChildElements)
                        {
                            workingStack.Push(childElement);
                        }
                    }
                }


                if (currentElement.SpecializationType is "DTO" or "Command" or "Query")
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

        [IntentManaged(Mode.Ignore)]
        private static IEnumerable<IElement> GetMappedEndpoints(ServiceProxyModel proxyModel)
        {
            if (proxyModel.Mapping?.Element?.IsServiceModel() == true)
            {
                IEnumerable<Services.Api.OperationModel> mappedOperations = proxyModel.MappedService.Operations;
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