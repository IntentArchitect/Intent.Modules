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
            var dtoModels = new List<ServiceProxyDTOModel>();
            foreach (var moduleModel in designer.GetServiceProxyModels())
            {
                dtoModels.AddRange(moduleModel.GetDTOModels());
            }

            return dtoModels.Distinct().ToList();
        }

        [IntentManaged(Mode.Ignore)]
        public static IEnumerable<ServiceProxyDTOModel> GetDTOModels(this ServiceProxyModel proxy)
        {
            var dtos = proxy.Operations
                .SelectMany(x => x.Parameters)
                .SelectMany(x => GetTypeModels(x.TypeReference))
                .Concat(proxy.Operations.Where(x => x.TypeReference.Element != null).SelectMany(x => GetTypeModels(x.TypeReference)))
                .ToList();

            foreach (var dto in dtos.ToList())
            {
                dtos.AddRange(((IElement)dto).GetChildDTOs());
            }

            return dtos
                .Where(x => x.SpecializationTypeId != TypeDefinitionModel.SpecializationTypeId &&
                            x.SpecializationTypeId != EnumModel.SpecializationTypeId)
                .Select(x => new ServiceProxyDTOModel((IElement)x, proxy)).ToList()
                .Distinct()
                .ToList();
        }

        [IntentManaged(Mode.Ignore)]
        private static IEnumerable<ICanBeReferencedType> GetTypeModels(ITypeReference typeReference)
        {
            var models = new List<ICanBeReferencedType>() { typeReference.Element };
            models.AddRange(typeReference.GenericTypeParameters.SelectMany(GetTypeModels));
            return models;
        }

        [IntentManaged(Mode.Ignore)]
        private static IEnumerable<IElement> GetChildDTOs(this IElement dto)
        {
            var childDTOs = dto.ChildElements
                .Where(x => x.TypeReference?.Element is IElement)
                .Select(x => (IElement)x.TypeReference.Element).ToList();
            foreach (var childDtO in childDTOs.ToList())
            {
                childDTOs.AddRange(GetChildDTOs(childDtO));
            }

            return childDTOs;
        }

    }
}