using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public static IDesigner Services(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.Services(application.Id);
        }

        public static IDesigner Services(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, "Services");
        }

    }
}