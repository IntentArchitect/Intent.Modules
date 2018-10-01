using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.IdentityServer.Templates.Clients
{
    public partial class IdentityServerClientsTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.IdentityServer.Clients";

        public IdentityServerClientsTemplate(IProject project, ISolutionEventDispatcher solutionEventDispatcher)
            : base (Identifier, project, null)
        {
            //solutionEventDispatcher.Subscribe(SolutionEvents.Authentication_ClientRequired, Handle);
        }

        //public ICollection<AuthenticationClient> Applications { get; } = new List<AuthenticationClient>();

        //private void Handle(SolutionEvent @event)
        //{
        //    Applications.Add(new AuthenticationClient(
        //        authenticationType: @event.GetValue("AuthenticationType"),
        //        applicationName: @event.GetValue("ApplicationName"), 
        //        applicationUrl: @event.GetValue("ApplicationUrl")));
        //}

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "Clients",
                fileExtension: "cs",
                defaultLocationInProject: "Config"
                );
        }
    }

    //public class AuthenticationClient
    //{
    //    public AuthenticationClient(string authenticationType, string applicationName, string applicationUrl)
    //    {
    //        AuthenticationType = authenticationType;
    //        ApplicationName = applicationName;
    //        ApplicationUrl = applicationUrl;
    //    }

    //    public string AuthenticationType { get; set; }
    //    public string ApplicationName { get; set; }
    //    public string ApplicationUrl { get; set; }
    //}
}
