using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.Angular.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<AngularServiceModel> GetAngularServiceModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(AngularServiceModel.SpecializationTypeId)
                .Select(x => new AngularServiceModel(x))
                .ToList();
        }

        public static IList<ComponentModel> GetComponentModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ComponentModel.SpecializationTypeId)
                .Select(x => new ComponentModel(x))
                .ToList();
        }

        public static IList<EnumModel> GetEnumModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(EnumModel.SpecializationTypeId)
                .Select(x => new EnumModel(x))
                .ToList();
        }

        public static IList<FolderModel> GetFolderModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(FolderModel.SpecializationTypeId)
                .Select(x => new FolderModel(x))
                .ToList();
        }

        public static IList<FormGroupDefinitionModel> GetFormGroupDefinitionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(FormGroupDefinitionModel.SpecializationTypeId)
                .Select(x => new FormGroupDefinitionModel(x))
                .ToList();
        }

        public static IList<ModelDefinitionModel> GetModelDefinitionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ModelDefinitionModel.SpecializationTypeId)
                .Select(x => new ModelDefinitionModel(x))
                .ToList();
        }

        public static IList<ModuleModel> GetModuleModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ModuleModel.SpecializationTypeId)
                .Select(x => new ModuleModel(x))
                .ToList();
        }

        public static IList<ServiceProxyModel> GetServiceProxyModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ServiceProxyModel.SpecializationTypeId)
                .Select(x => new ServiceProxyModel(x))
                .ToList();
        }

        public static IList<TypeDefinitionModel> GetTypeDefinitionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
                .Select(x => new TypeDefinitionModel(x))
                .ToList();
        }

    }
}