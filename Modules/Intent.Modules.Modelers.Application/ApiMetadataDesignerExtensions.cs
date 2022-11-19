using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modelers.Application.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public static IDesigner Application(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.Application(application.Id);
        }

        public static IDesigner Application(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, "Application");
        }

    }
}