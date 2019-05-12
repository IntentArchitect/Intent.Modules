using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services;
using Intent.Modules.Angular.Api;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.Angular
{
    public static class MetadataManagerExtensions
    {
        public static IEnumerable<IModuleModel> GetModules(this IMetadataManager metadataManager, IApplication application)
        {
            return new MetadataProvider(metadataManager).GetModules(application);
        }

        public static IEnumerable<IModuleDTOModel> GetModels(this IMetadataManager metadataManager, Engine.IApplication application)
        {
            var dtoModels = new List<IModuleDTOModel>();
            foreach (var moduleModel in metadataManager.GetModules(application))
            {
                dtoModels.AddRange(moduleModel.GetModels());
            }

            return dtoModels.Distinct().ToList();
        }

        public static IEnumerable<IModuleDTOModel> GetModels(this IModuleModel moduleModel)
        {
            var operations = moduleModel.ServiceProxies
                .SelectMany(x => x.Operations).ToList();
            var classes = operations
                .SelectMany(x => x.Parameters)
                .SelectMany(x => GetTypeModels(x.Type))
                .Concat(operations.Where(x => x.ReturnType != null).SelectMany(x => GetTypeModels(x.ReturnType.Type)));

            return moduleModel.ModelDefinitions.Concat(classes.Where(x => x.IsDTO()).Select(x => new ModuleDTOModel(x, moduleModel)).ToList())
                .Distinct()
                .ToList();
        }

        private static IEnumerable<IClass> GetTypeModels(ITypeReference typeReference)
        {
            var models = new List<IClass>() { typeReference.Model };
            models.AddRange(typeReference.GenericTypeParameters.SelectMany(GetTypeModels));
            return models;
        }
    }
}