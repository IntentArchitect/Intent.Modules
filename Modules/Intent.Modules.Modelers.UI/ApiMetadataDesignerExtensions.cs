using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public const string UserInterfaceDesignerId = "f492faed-0665-4513-9853-5a230721786f";
        public static IDesigner UserInterface(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.UserInterface(application.Id);
        }

        public static IDesigner UserInterface(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, UserInterfaceDesignerId);
        }

    }
}