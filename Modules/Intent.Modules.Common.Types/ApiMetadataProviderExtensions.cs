using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.Common.Types.Api
{
    public static class ApiMetadataProviderExtensions
    {
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

        public static IList<TypeDefinitionModel> GetTypeDefinitionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
                .Select(x => new TypeDefinitionModel(x))
                .ToList();
        }

    }
}