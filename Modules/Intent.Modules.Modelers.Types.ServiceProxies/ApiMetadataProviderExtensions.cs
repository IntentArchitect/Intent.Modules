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
        public static IReadOnlyCollection<EnumModel> GetProxyMappedEnumModels(this IDesigner designer)
        {
            return designer.GetServiceProxyModels()
                .SelectMany(x => x.GetMappedServiceDTOModels())
                .SelectMany(x => x.Fields)
                .Where(x => x.TypeReference.Element?.IsEnumModel() == true)
                .Select(x => x.TypeReference.Element.AsEnumModel())
                .Distinct()
                .ToArray();
        }

        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<ServiceProxyDTOModel> GetDTOModels(this ServiceProxyModel proxy)
        {
            var operationLevelDtos = proxy.Operations
                .SelectMany(x => x.Parameters)
                .SelectMany(x => GetTypesFromTypeReference(x.TypeReference))
                .Concat(proxy.Operations
                    .Where(x => x.TypeReference?.Element != null)
                    .SelectMany(x => GetTypesFromTypeReference(x.TypeReference)));

            var detectedDtos = GetDistinctAndNestedDtos(operationLevelDtos);

            return detectedDtos
                .Where(x => x.SpecializationTypeId != TypeDefinitionModel.SpecializationTypeId
                            && x.SpecializationTypeId != EnumModel.SpecializationTypeId)
                .Select(x => new ServiceProxyDTOModel((IElement)x, proxy))
                .ToList();
        }

        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<ServiceProxyDTOModel> GetMappedServiceDTOModels(this ServiceProxyModel proxy)
        {
            var operationLevelDtos = proxy.MappedService.Operations
                .SelectMany(x => x.Parameters)
                .SelectMany(x => GetTypesFromTypeReference(x.TypeReference))
                .Concat(proxy.MappedService.Operations
                    .Where(x => x.TypeReference?.Element != null)
                    .SelectMany(x => GetTypesFromTypeReference(x.TypeReference)));

            var detectedDtos = GetDistinctAndNestedDtos(operationLevelDtos);

            return detectedDtos
                .Where(x => x.SpecializationTypeId != TypeDefinitionModel.SpecializationTypeId
                            && x.SpecializationTypeId != EnumModel.SpecializationTypeId)
                .Select(x => new ServiceProxyDTOModel((IElement)x, proxy))
                .ToList();
        }

        [IntentManaged(Mode.Ignore)]
        private static IEnumerable<ICanBeReferencedType> GetTypesFromTypeReference(ITypeReference typeReference)
        {
            var models = new List<ICanBeReferencedType>() { typeReference.Element };
            models.AddRange(typeReference.GenericTypeParameters.SelectMany(GetTypesFromTypeReference));
            return models;
        }

        [IntentManaged(Mode.Ignore)]
        private static IEnumerable<ICanBeReferencedType> GetDistinctAndNestedDtos(
            IEnumerable<ICanBeReferencedType> operationLevelDtos)
        {
            var detectedDtos = new HashSet<ICanBeReferencedType>(operationLevelDtos);
            var navigationStack = new Stack<ICanBeReferencedType>(detectedDtos);

            while (navigationStack.Any())
            {
                var curDto = navigationStack.Pop();
                var elements = ((IElement)curDto).ChildElements
                    .Where(x => x.TypeReference?.Element is IElement)
                    .Select(x => (IElement)x.TypeReference.Element);
                foreach (var element in elements)
                {
                    // Also helps not to get stuck in circular references
                    if (detectedDtos.Contains(element))
                    {
                        continue;
                    }

                    detectedDtos.Add(element);
                    navigationStack.Push(element);
                }
            }

            return detectedDtos;
        }
    }
}