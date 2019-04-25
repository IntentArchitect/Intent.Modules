using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Logging.NLog.Interop.WebApi.Decorators;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Logging.NLog.Interop.WebApi
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            RegisterDecorator<WebApiControllerDecoratorBase>(NLogWebApiControllerDecorator.IDENTIFIER, new NLogWebApiControllerDecorator());
        }
    }
}
