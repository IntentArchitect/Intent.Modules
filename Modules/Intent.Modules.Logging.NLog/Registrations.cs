using Intent.Modules.Common.Registrations;
using Intent.Modules.Logging.NLog.Decorators;
using Intent.Modules.Logging.NLog.Templates.OperationRequestId;
using Intent.Modules.Logging.NLog.Templates.OperationRequestIdRenderer;
using Intent.Modules.Logging.NLog.Templates.SanitizingJsonSerializer;
using Intent.Modules.VisualStudio.Projects.Decorators;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Logging.NLog
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetadataManager metaDataManager)
        {
            RegisterDecorator<IWebConfigDecorator>(NLogWebConfigDecorator.Identifier, new NLogWebConfigDecorator());

            RegisterTemplate(OperationRequestIdTemplate.Identifier, project => new OperationRequestIdTemplate(project));
            RegisterTemplate(NLogOperationRequestIdRendererTemplate.Identifier, project => new NLogOperationRequestIdRendererTemplate(project));
            RegisterTemplate(SanitizingJsonSerializerTemplate.Identifier, project => new SanitizingJsonSerializerTemplate(project));
        }
    }
}
