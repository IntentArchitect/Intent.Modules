using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ComponentGroupModel> GetComponentGroupModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ComponentGroupModel.SpecializationTypeId)
                .Select(x => new ComponentGroupModel(x))
                .ToList();
        }

        public static IList<ContentFolderModel> GetContentFolderModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ContentFolderModel.SpecializationTypeId)
                .Select(x => new ContentFolderModel(x))
                .ToList();
        }

        public static IList<InstallationSettingsModel> GetInstallationSettingsModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(InstallationSettingsModel.SpecializationTypeId)
                .Select(x => new InstallationSettingsModel(x))
                .ToList();
        }

    }
}