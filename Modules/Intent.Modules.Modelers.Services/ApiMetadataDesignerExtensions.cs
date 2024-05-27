using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public const string ServicesDesignerId = "81104ae6-2bc5-4bae-b05a-f987b0372d81";
        public static IDesigner Services(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.Services(application.Id);
        }

        public static IDesigner Services(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, ServicesDesignerId);
        }

    }
}