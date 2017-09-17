using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

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
            return new RoslynMergeConfig(new TemplateMetaData(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "IClientNotificationService",
                fileExtension: "cs",
                defaultLocationInProject: "Services",
                className: "IClientNotificationService",
                @namespace: "${Project.ProjectName}.Services");
        }
    }
}
