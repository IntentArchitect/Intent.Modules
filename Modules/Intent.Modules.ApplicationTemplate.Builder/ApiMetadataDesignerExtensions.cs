using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public static IDesigner AppTemplates(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.AppTemplates(application.Id);
        }

        public static IDesigner AppTemplates(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, "App Templates");
        }

    }
}