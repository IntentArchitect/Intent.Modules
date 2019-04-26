using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;


namespace Intent.Modules.AspNet.SignalR.Templates.ClientNotificationService
{
    public class ClientNotificationServiceRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => ClientNotificationService.Identifier;
        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new ClientNotificationService(project);
        }
    }
}
