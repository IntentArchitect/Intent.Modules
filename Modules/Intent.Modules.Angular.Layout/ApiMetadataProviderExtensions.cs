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
        public static IList<ButtonControlModel> GetButtonControlModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetButtonControlModels(application);
        }

        public static IList<FormModel> GetFormModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFormModels(application);
        }

        public static IList<FormControlModel> GetFormControlModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetFormControlModels(application);
        }

        public static IList<SectionModel> GetSectionModels(this IMetadataManager metadataManager, IApplication application)
        {
            return new ApiMetadataProvider(metadataManager).GetSectionModels(application);
        }

    }
}