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
            var mappedEndpoints = designer.GetServiceProxyModels()
                .SelectMany(GetMappedEndpoints);

            return DeepGetDistinctReferencedElements(mappedEndpoints)
                .Where(x => x.IsEnumModel())
                .Select(x => x.AsEnumModel())
                .ToList();
        }

        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<ServiceProxyDTOModel> GetDTOModels(this ServiceProxyModel proxy)
        {
            return DeepGetDistinctReferencedElements(proxy.Operations.Select(x => x.InternalElement))
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
        private static ISet<IElement> DeepGetDistinctReferencedElements(IEnumerable<IElement> elements)
        {
            var referencedElements = new HashSet<IElement>();
            var workingStack = new Stack<IElement>(elements);

            while (workingStack.Any())
            {
                var currentElement = workingStack.Pop();
                if (currentElement.TypeReference?.Element is IElement referencedElement)
                {
                    if (!referencedElements.Add(referencedElement))
                    {
                        // Avoid infinite loops due to cyclic references
                        continue;
                    }

                    foreach (var childElement in referencedElement.ChildElements)
                    {
                        workingStack.Push(childElement);
                    }
                }

                foreach (var childElement in currentElement.ChildElements)
                {
                    workingStack.Push(childElement);
                }
            }

            return referencedElements;
        }

        [IntentManaged(Mode.Ignore)]
        private static IEnumerable<IElement> GetMappedEndpoints(ServiceProxyModel model)
        {
            if (model.MappedService != null)
            {
                return model.MappedService.Operations
                    .Select(x => x.InternalElement)
                    .Where(x => x.Stereotypes.Any(s => s.Name == "Http Settings"));
            }

            return model.Operations
                .Select(x => x.Mapping?.Element)
                .Cast<IElement>()
                .Where(x => x.Stereotypes.Any(s => s.Name == "Http Settings"));
        }
    }
}