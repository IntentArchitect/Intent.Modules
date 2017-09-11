using Intent.Packages.UserContext.Templates.UserContextInterface;
using Intent.Packages.UserContext.Templates.UserContextProvider;
using Intent.Packages.UserContext.Templates.UserContextStatic;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.UserContext
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterTemplate(UserContextInterfaceTemplate.Identifier, project => new UserContextInterfaceTemplate(project));
            RegisterTemplate(UserContextStaticTemplate.Identifier, project => new UserContextStaticTemplate(project));
            RegisterTemplate(UserContextProviderTemplate.Identifier, project => new UserContextProviderTemplate(project, application.EventDispatcher));
        }
    }
}
