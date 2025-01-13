using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    public static class ApiMetadataProviderExtensions
    {

        public static IList<ComponentModel> GetComponentModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ComponentModel.SpecializationTypeId)
                .Select(x => new ComponentModel(x))
                .ToList();
        }

        public static IList<ComponentViewModel> GetComponentViewModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ComponentViewModel.SpecializationTypeId)
                .Select(x => new ComponentViewModel(x))
                .ToList();
        }

        public static IList<DiagramModel> GetDiagramModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(DiagramModel.SpecializationTypeId)
                .Select(x => new DiagramModel(x))
                .ToList();
        }

        public static IList<LayoutModel> GetLayoutModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(LayoutModel.SpecializationTypeId)
                .Select(x => new LayoutModel(x))
                .ToList();
        }

        public static IList<ModelDefinitionModel> GetModelDefinitionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ModelDefinitionModel.SpecializationTypeId)
                .Select(x => new ModelDefinitionModel(x))
                .ToList();
        }

        public static IList<ServiceModel> GetServiceModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ServiceModel.SpecializationTypeId)
                .Select(x => new ServiceModel(x))
                .ToList();
        }

    }
}