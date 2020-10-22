using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ButtonControlModel> GetButtonControlModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ButtonControlModel.SpecializationTypeId)
                .Select(x => new ButtonControlModel(x))
                .ToList();
        }

        public static IList<FormModel> GetFormModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(FormModel.SpecializationTypeId)
                .Select(x => new FormModel(x))
                .ToList();
        }

        public static IList<FormControlModel> GetFormControlModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(FormControlModel.SpecializationTypeId)
                .Select(x => new FormControlModel(x))
                .ToList();
        }

        public static IList<SectionModel> GetSectionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(SectionModel.SpecializationTypeId)
                .Select(x => new SectionModel(x))
                .ToList();
        }

        public static IList<TabsModel> GetTabsModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(TabsModel.SpecializationTypeId)
                .Select(x => new TabsModel(x))
                .ToList();
        }

    }
}