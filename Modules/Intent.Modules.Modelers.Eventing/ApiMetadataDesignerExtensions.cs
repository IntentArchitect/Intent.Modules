using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public const string EventingDesignerId = "822e4254-9ced-4dd1-ad56-500b861f7e4d";
        public static IDesigner Eventing(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.Eventing(application.Id);
        }

        public static IDesigner Eventing(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, EventingDesignerId);
        }

    }
}