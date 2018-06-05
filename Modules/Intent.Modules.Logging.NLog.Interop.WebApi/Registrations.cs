using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Logging.NLog.Interop.WebApi.Decorators;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Logging.NLog.Interop.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<WebApiControllerDecoratorBase>(NLogWebApiControllerDecorator.Identifier, new NLogWebApiControllerDecorator());
        }
    }
}
