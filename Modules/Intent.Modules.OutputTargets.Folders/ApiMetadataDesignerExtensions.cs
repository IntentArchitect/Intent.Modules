using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions", Version = "1.0")]

namespace Intent.Modules.OutputTargets.Folders.Api
{
    public static class ApiMetadataDesignerExtensions
    {
        public static IDesigner FolderStructure(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.FolderStructure(application.Id);
        }

        public static IDesigner FolderStructure(this IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetDesigner(applicationId, "Folder Structure");
        }

    }
}