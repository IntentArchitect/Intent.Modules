using System.Linq;
using Intent.Packages.Logging.NLog.Decorators;
using Intent.Packages.Logging.NLog.Templates.OperationRequestId;
using Intent.Packages.Logging.NLog.Templates.OperationRequestIdRenderer;
using Intent.Packages.Logging.NLog.Templates.SanitizingJsonSerializer;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.VSProjects.Decorators;

namespace Intent.Package.Logging.NLog
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterDecorator<IWebConfigDecorator>(NLogWebConfigDecorator.Identifier, new NLogWebConfigDecorator());

            RegisterTemplate(OperationRequestIdTemplate.Identifier, project => new OperationRequestIdTemplate(project));
            RegisterTemplate(NLogOperationRequestIdRendererTemplate.Identifier, project => new NLogOperationRequestIdRendererTemplate(project));
            RegisterTemplate(SanitizingJsonSerializerTemplate.Identifier, project => new SanitizingJsonSerializerTemplate(project));
        }
    }
}
