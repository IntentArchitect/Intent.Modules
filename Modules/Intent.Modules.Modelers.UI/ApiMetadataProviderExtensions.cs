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
        public static IList<CheckboxModel> GetCheckboxModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(CheckboxModel.SpecializationTypeId)
                .Select(x => new CheckboxModel(x))
                .ToList();
        }
        public static IList<ComponentModel> GetComponentModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ComponentModel.SpecializationTypeId)
                .Select(x => new ComponentModel(x))
                .ToList();
        }

        public static IList<ContainerModel> GetContainerModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ContainerModel.SpecializationTypeId)
                .Select(x => new ContainerModel(x))
                .ToList();
        }

        public static IList<FormModel> GetFormModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(FormModel.SpecializationTypeId)
                .Select(x => new FormModel(x))
                .ToList();
        }

        public static IList<ModelDefinitionModel> GetModelDefinitionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ModelDefinitionModel.SpecializationTypeId)
                .Select(x => new ModelDefinitionModel(x))
                .ToList();
        }

        public static IList<TableModel> GetTableModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TableModel.SpecializationTypeId)
                .Select(x => new TableModel(x))
                .ToList();
        }

        public static IList<TextModel> GetTextModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TextModel.SpecializationTypeId)
                .Select(x => new TextModel(x))
                .ToList();
        }

        public static IList<TextInputModel> GetTextInputModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TextInputModel.SpecializationTypeId)
                .Select(x => new TextInputModel(x))
                .ToList();
        }

    }
}