using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

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
