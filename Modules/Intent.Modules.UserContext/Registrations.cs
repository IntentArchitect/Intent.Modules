using Intent.Modules.Common.Registrations;
using Intent.Modules.UserContext.Templates.UserContextInterface;
using Intent.Modules.UserContext.Templates.UserContextProvider;
using Intent.Modules.UserContext.Templates.UserContextProviderInterface;
using Intent.Modules.UserContext.Templates.UserContextStatic;
using Intent.Engine;
using Intent.Registrations;

namespace Intent.Modules.UserContext
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetadataManager metadataManager)
        {
            RegisterTemplate(UserContextInterfaceTemplate.Identifier, project => new UserContextInterfaceTemplate(project));
            RegisterTemplate(UserContextStaticTemplate.Identifier, project => new UserContextStaticTemplate(project));
            RegisterTemplate(UserContextProviderInterfaceTemplate.Identifier, project => new UserContextProviderInterfaceTemplate(project));
            RegisterTemplate(UserContextProviderTemplate.Identifier, project => new UserContextProviderTemplate(project, application.EventDispatcher));
        }
    }
}
