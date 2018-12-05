using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.Modules.UserContext.Interop.AspNet.WebApi.Decorators;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.UserContext.Interop.AspNet.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<WebApiControllerDecoratorBase>(UserContextWebApiControllerDecorator.Identifier, new UserContextWebApiControllerDecorator());
        }
    }
}
