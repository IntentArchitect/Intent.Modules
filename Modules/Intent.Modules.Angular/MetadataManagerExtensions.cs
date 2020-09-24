using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Angular.Api;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.Angular
{
    public static class MetadataManagerExtensions
    {
        public static IEnumerable<ModuleDTOModel> GetDTOModels(this IMetadataManager metadataManager, IApplication application)
        {
            var dtoModels = new List<ModuleDTOModel>();
            foreach (var moduleModel in metadataManager.Angular(application).GetModuleModels())
            {
                dtoModels.AddRange(moduleModel.GetDTOModels());
            }

            return dtoModels.Distinct().ToList();
        }

        public static IEnumerable<ModuleDTOModel> GetDTOModels(this ModuleModel moduleModel)
        {
            var operations = moduleModel.ServiceProxies
                .SelectMany(x => x.Operations).ToList();
            var classes = operations
                .SelectMany(x => x.Parameters)
                .SelectMany(x => GetTypeModels(x.TypeReference))
                .Concat(operations.Where(x => x.ReturnType != null).SelectMany(x => GetTypeModels(x.ReturnType)));

            return classes.Where(x => x.SpecializationType == DTOModel.SpecializationType).Select(x => new ModuleDTOModel((IElement) x, moduleModel)).ToList()
                .Distinct()
                .ToList();
        }

        private static IEnumerable<ICanBeReferencedType> GetTypeModels(ITypeReference typeReference)
        {
            var models = new List<ICanBeReferencedType>() { typeReference.Element };
            models.AddRange(typeReference.GenericTypeParameters.SelectMany(GetTypeModels));
            return models;
        }
    }
}