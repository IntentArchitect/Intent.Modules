using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

// Testing my commit
namespace Intent.ModuleBuilder.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public static IDesigner ModuleBuilder(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.ModuleBuilder(application.Id);
        }

        public static IDesigner ModuleBuilder(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, "Module Builder");
        }

    }
}