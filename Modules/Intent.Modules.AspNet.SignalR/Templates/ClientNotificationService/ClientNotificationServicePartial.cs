using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;

namespace Intent.Modules.AspNet.SignalR.Templates.ClientNotificationService
{
    public partial class ClientNotificationService : IntentRoslynProjectItemTemplateBase
    {
        public const string Identifier = "Intent.AspNet.SignalR.IClientNotificationService";

        public ClientNotificationService(IProject project) : base(Identifier, project)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "IClientNotificationService",
                fileExtension: "cs",
                defaultLocationInProject: "Services",
                className: "IClientNotificationService",
                @namespace: "${Project.ProjectName}.Services");
        }
    }
}
