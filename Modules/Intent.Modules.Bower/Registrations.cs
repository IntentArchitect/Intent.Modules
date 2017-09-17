using Intent.Modules.Bower.Templates.BowerConfig;
using Intent.Modules.Bower.Templates.BowerRCFile;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Bower
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            RegisterTemplate(BowerConfigTemplate.Identifier, project => new BowerConfigTemplate(project));
            RegisterTemplate(BowerRCFileTemplate.Identifier, project => new BowerRCFileTemplate(project));
        }
    }
}
